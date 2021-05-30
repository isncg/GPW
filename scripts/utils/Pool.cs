using System.Collections.Generic;

namespace GPW
{
	public static class Pool<T> where T : new()
	{
		static Queue<T> recycleQueue = new Queue<T>();
		public static T Alloc(bool enableNew = true)
		{
			if (recycleQueue.Count > 0)
				return recycleQueue.Dequeue();
			if (enableNew)
				return new T();
			return default(T);
		}

		public static void Recycle(T obj)
		{
			recycleQueue.Enqueue(obj);
		}
	}
}