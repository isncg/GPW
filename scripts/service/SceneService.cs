using System;
using System.Collections.Generic;

namespace GPW
{
	public enum SceneType
	{
		None,
		Launching,
		Lobby,
		BattleLoading,
		Battle,
		Settlement
	}
	public class SceneService : Service<SceneService>
	{
		public Dictionary<int, Scene> configs = new Dictionary<int, Scene>();
		Scene curScene = null;
		SceneType curSceneType = SceneType.None;

		public override void Init()
		{
			base.Init();
			configs[(int)SceneType.Launching] = new SceneLaunching();
			configs[(int)SceneType.Lobby] = new SceneLobby();
		}

		public override void Update()
		{
			base.Update();
			if (null != curScene)
				curScene.OnUpdate();
		}

		public void RunScene(SceneType sceneType)
		{
			if (sceneType != SceneType.None && sceneType != curSceneType)
			{
				if (configs.TryGetValue((int)sceneType, out var sc))
				{
					if (null != curScene)
						curScene.BeforeLeave();
					sc.OnEnter();
					curSceneType = sceneType;
					if (null != curScene)
						curScene.AfterLeave();
					curScene = sc;
				}
			}
		}
	}
}