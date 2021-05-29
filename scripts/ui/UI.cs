using Godot;
using GPW;
using System;

namespace GPW
{
	public class UI : Node2D
	{
		public void OnShow(object param)
		{

		}

		public void OnClose()
		{

		}

		public void RegisterEvent(EventType eventType, Action<object> callback)
		{
			EventService.Instance.Register(this, eventType, callback);
		}

		public void UnRegisterEvent(EventType eventType)
		{
			EventService.Instance.UnRegister(this, eventType);
		}
	}
}