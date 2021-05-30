namespace GPW
{
	public class SceneLobby : Scene
	{
		public override void OnEnter()
		{
			base.OnEnter();
			UIService.Instance.Show(UIID.UIMainMenu);
		}
	}
}