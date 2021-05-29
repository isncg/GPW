using Godot;
using System;

namespace GPW
{
	public interface IService
	{
		void Init();
		void Reset();
		void Update();
	}

	public abstract class Service<T> : IService where T : new()
	{
		private static T inst = default(T);
		public static T Instance
		{
			get
			{
				if (null != inst) return inst;
				return inst = new T();
			}
		}

		public virtual void Init() { }
		public virtual void Reset() { }
		public virtual void Update() { }
	}
}
