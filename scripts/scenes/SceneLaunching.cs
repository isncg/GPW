namespace GPW
{
	public class SceneLaunching : Scene
	{
		public int progress = 0;
		public override void OnEnter()
		{
			base.OnEnter();
			progress = 0;
			UIService.Instance.Show(UIType.UILaunching);
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			progress++;
			if (progress > 100)
				SceneService.Instance.RunScene(SceneType.Lobby);
			EventService.Instance.Dispatch(EventType.UI_LOADING_PROGRESS_UPDATE, progress);
		}

		public override void AfterLeave()
		{
			base.AfterLeave();
			UIService.Instance.Close(UIType.UILaunching);
		}
	}
}