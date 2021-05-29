using Godot;
using System;
using System.Collections.Generic;
namespace GPW
{
	public enum EventType
	{
		NONE,
		UI_LOADING_PROGRESS_UPDATE,
	}
	public class EventService : Service<EventService>
	{

		public Dictionary<int, Dictionary<object, Action<object>>> callbacks =
			new Dictionary<int, Dictionary<object, Action<object>>>();

		public void Register(object user, EventType type, Action<object> callback)
		{
			Dictionary<object, Action<object>> userCallbackDict = null;
			if (!callbacks.TryGetValue((int)type, out userCallbackDict))
			{
				userCallbackDict = new Dictionary<object, Action<object>>();
				callbacks[(int)type] = userCallbackDict;
			}
			userCallbackDict[user] = callback;
		}

		public void UnRegister(object user, EventType type)
		{
			if (callbacks.TryGetValue((int)type, out var userCallbackDict))
				userCallbackDict.Remove(user);
		}

		public void Dispatch(EventType type, object e)
		{
			if (callbacks.TryGetValue((int)type, out var userCallbackDict))
				foreach (var cb in userCallbackDict.Values)
					cb(e);
		}
	}
}
