using Godot;
using Godot.Collections;

namespace GPW
{

	public enum ButtonID
	{
		StartBattle,
		Exit
	}

	public class GpwButton : Button
	{
		[Export] public ButtonID buttonID;
		public delegate void OnClickHandler();
		public event OnClickHandler onClick;
		public override void _Pressed()
		{
			base._Pressed();
			onClick?.Invoke();
			EventService.Instance.Dispatch(EventType.UI_BUTTONID, buttonID);
		}
	}
}