namespace GPW
{
	public class SceneLaunching : Scene
	{
		public int progress = 0;
		public override void OnEnter()
		{
			base.OnEnter();
			progress = 0;
			UIService.Instance.Show(UIID.UILaunching);
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			progress++;
			if (progress > 100)
				SceneService.Instance.RunScene(SceneType.Lobby);
			else
				EventService.Instance.Dispatch(EventType.UI_LOADING_PROGRESS_UPDATE, progress, this);
		}

		public override void AfterLeave()
		{
			base.AfterLeave();
			UIService.Instance.Close(UIID.UILaunching);
		}
	}
}