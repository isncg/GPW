using Godot;

namespace GPW
{
	public class SceneBattle : Scene
	{
		public override void AfterLeave()
		{
			base.AfterLeave();
		}

		public override void BeforeLeave()
		{
			base.BeforeLeave();
			BattleService.Instance.ReleaseBattleRoot();
		}

		public override void OnEnter()
		{
			base.OnEnter();
			UIService.Instance.CloseAll();
			BattleService.Instance.InitBattleRoot();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
		}
		public override void OnInput(InputEvent inputEvent)
		{
			if (inputEvent is InputEventKey keyEvent && keyEvent.Pressed)
			{
				if (keyEvent.Scancode == (uint)KeyList.Escape)
					SceneService.Instance.RunScene(SceneType.Lobby);
			}
			base.OnInput(inputEvent);
		}
	}
}