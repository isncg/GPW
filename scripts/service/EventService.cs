using Godot;
using System;
using System.Collections.Generic;
namespace GPW
{
	public enum EventType
	{
		NONE,
		UI_BUTTONID,
		UI_LOADING_PROGRESS_UPDATE,
	}



	public class EventService : Service<EventService>
	{
		// EventType - userObj - handler
		public Dictionary<int, Dictionary<object, Action<object, object>>> typeDict =
			new Dictionary<int, Dictionary<object, Action<object, object>>>();

		public void Register(object user, EventType type, Action<object, object> handler)
		{
			Dictionary<object, Action<object, object>> userDict = null;
			if (!typeDict.TryGetValue((int)type, out userDict))
			{
				userDict = new Dictionary<object, Action<object, object>>();
				typeDict[(int)type] = userDict;
			}
			userDict[user] = handler;
		}

		public void UnRegister(object user, EventType type)
		{
			if (typeDict.TryGetValue((int)type, out var userCallbackDict))
				userCallbackDict.Remove(user);
		}

		public void Dispatch(EventType type, object arg = null, object sender = null)
		{
			if (typeDict.TryGetValue((int)type, out var userCallbackDict))
				foreach (var cb in userCallbackDict.Values)
					cb(arg, sender);
		}
	}
}
