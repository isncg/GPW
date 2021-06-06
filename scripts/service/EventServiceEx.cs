using System;
using System.Collections.Generic;

namespace GPW
{
	public class EventServiceEx : Service<EventServiceEx>
	{
		public Dictionary<Type, Dictionary<object, object>> type2userregisters;

		public void Register<EventEnumType, SenderType, ArgType>(
			EventEnumType type,
			object user,
			EventHandlers<EventEnumType, SenderType, ArgType>.Handler handler) where EventEnumType : Enum
		{
			var registerType = typeof(EventRegister<EventEnumType, SenderType, ArgType>);
			Dictionary<object, object> user2reg = null;
			if (!type2userregisters.TryGetValue(registerType, out user2reg))
			{
				user2reg = new Dictionary<object, object>();
				type2userregisters[registerType] = user2reg;
			}

			object regsobj = null;
			if (!user2reg.TryGetValue(user, out regsobj))
			{
				var eventRegister = new EventRegister<EventEnumType, SenderType, ArgType>();
				eventRegister.Register(type, handler);
				user2reg[user] = eventRegister;
			}
			else if (regsobj is EventRegister<EventEnumType, SenderType, ArgType> obj)
				obj.Register(type, handler);
		}

		public void UnRegister<EventEnumType, SenderType, ArgType>(
			EventEnumType type,
			object user,
			EventHandlers<EventEnumType, SenderType, ArgType>.Handler handler) where EventEnumType : Enum
		{
			var registerType = typeof(EventRegister<EventEnumType, SenderType, ArgType>);
			Dictionary<object, object> user2reg = null;
			if (!type2userregisters.TryGetValue(registerType, out user2reg)) return;
			object regsobj = null;
			if (!user2reg.TryGetValue(user, out regsobj)) return;
			if (regsobj is EventRegister<EventEnumType, SenderType, ArgType> obj)
				obj.UnRegister(type, handler);
		}

		public void Dispatch<EventEnumType, SenderType, ArgType>(
			EventEnumType type,
			SenderType sender,
			ArgType arg,
			object user,
			EventHandlers<EventEnumType, SenderType, ArgType>.Handler handler) where EventEnumType : Enum
		{
			var registerType = typeof(EventRegister<EventEnumType, SenderType, ArgType>);
			Dictionary<object, object> user2reg = null;
			if (!type2userregisters.TryGetValue(registerType, out user2reg)) return;
			object regsobj = null;
			if (!user2reg.TryGetValue(user, out regsobj)) return;
			if (regsobj is EventRegister<EventEnumType, SenderType, ArgType> obj)
				obj.Dispatch(type, sender, arg);
		}
	}


	public class EventHandlers<EventEnumType, SenderType, ArgType> where EventEnumType : Enum
	{
		public delegate void Handler(SenderType senderType, ArgType argType);
		public event Handler handlers;
		public void Dispatch(SenderType sender, ArgType arg)
		{
			handlers?.Invoke(sender, arg);
		}


	}

	public class EventRegister<EventEnumType, SenderType, ArgType> where EventEnumType : Enum
	{
		private int INT(EventEnumType type) => Convert.ToInt32(type);
		public Dictionary<int, EventHandlers<EventEnumType, SenderType, ArgType>> handlerDict = new Dictionary<int, EventHandlers<EventEnumType, SenderType, ArgType>>();
		public void Register(EventEnumType type, EventHandlers<EventEnumType, SenderType, ArgType>.Handler handler)
		{
			if (handlerDict.TryGetValue(INT(type), out var handlers))
				handlers.handlers += handler;
		}
		public void UnRegister(EventEnumType type, EventHandlers<EventEnumType, SenderType, ArgType>.Handler handler)
		{
			if (handlerDict.TryGetValue(INT(type), out var handlers))
				handlers.handlers -= handler;
		}

		public void Dispatch(EventEnumType type, SenderType sender, ArgType arg)
		{
			if (handlerDict.TryGetValue(INT(type), out var handlers))
				handlers.Dispatch(sender, arg);
		}
	}

}