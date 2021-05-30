using System.Collections;
using System.Collections.Generic;

namespace GPW
{
	public class Coroutine
	{
		public Stack<IEnumerator> enumerators = new Stack<IEnumerator>();
		public void Start(IEnumerator routine)
		{
			enumerators.Push(routine);
		}

		public void Stop()
		{
			enumerators.Clear();
		}

		public void Next()
		{
			if (enumerators.Count > 0)
			{
				var top = enumerators.Peek();
				if (!top.MoveNext())
					enumerators.Pop();
			}
		}
	}
}