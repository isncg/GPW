using Godot;
using System;

namespace GPW
{
	public interface IService
	{
		void Init();
		void Reset();
		void Update();
		void Input(InputEvent inputEvent);
	}

	public abstract class Service<T> : IService where T : new()
	{
		private static T inst = default(T);
		public static T Instance // for C# 8.0 => inst??(inst = inst = new T());
		{
			get
			{
				if (null != inst) return inst;
				Log.I("[Service:Instance] new {0}", typeof(T));
				return inst = new T();
			}
		}

		public virtual void Init() { }
		public virtual void Reset() { }
		public virtual void Update() { }
		public virtual void Input(InputEvent inputEvent) { }
	}
}
