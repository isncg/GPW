using Godot;
using GPW;
public class UIMainMenu : UI
{
	GpwButton btnSinglePlayer;
	GpwButton btnMultiPlayer;
	GpwButton btnOption;
	GpwButton btnAbout;
	GpwButton btnExit;
	public override void OnShow(object param)
	{
		base.OnShow(param);
		btnSinglePlayer = GetNode<GpwButton>("MenuRoot/Content/Button0");
		btnMultiPlayer = GetNode<GpwButton>("MenuRoot/Content/Button1");
		btnOption = GetNode<GpwButton>("MenuRoot/Content/Button2");
		btnAbout = GetNode<GpwButton>("MenuRoot/Content/Button3");
		btnExit = GetNode<GpwButton>("MenuRoot/Content/Button4");

		if (null != btnExit)
			btnExit.onClick += OnClickExit;
	}

	private void OnClickExit()
	{
		GetTree().Quit(0);
	}
}