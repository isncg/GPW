using Godot;
using Godot.Collections;

namespace GPW
{
	public class GpwButton : Button
	{
		public delegate void OnClickHandler();
		public event OnClickHandler onClick;
		public override void _Pressed()
		{
			base._Pressed();
			onClick?.Invoke();
		}
	}
}