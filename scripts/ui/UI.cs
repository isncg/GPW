using Godot;
using GPW;
using System;
using System.Collections;
using System.Collections.Generic;
namespace GPW
{
	public class UI : Node2D
	{
		private readonly Coroutine coroutine = new Coroutine();
		public virtual void OnShow(object param) { }
		//public virtual void OnClose() { }
		public virtual void OnDestroy() { }
		public override void _Process(float delta)
		{
			coroutine.Next();
		}

		public void RegisterEvent(EventType eventType, Action<object, object> callback)
		{
			EventService.Instance.Register(this, eventType, callback);
		}

		public void UnRegisterEvent(EventType eventType)
		{
			EventService.Instance.UnRegister(this, eventType);
		}

		protected void StartCoroutine(IEnumerator routine)
		{
			if (null != routine)
				coroutine.Start(routine);
		}

		protected void StopCoroutine()
		{
			coroutine.Stop();
		}
		public virtual bool OnClose() => true;
	}
}