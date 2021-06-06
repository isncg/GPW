using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
namespace GPW
{
	public enum UIID : int
	{
		UILaunching = 10001,
		UIMainMenu = 10002,
		UIBattleLoading = 10003,
		UIBattleStart = 10004,
		UIBattleFinish = 10005,
		UISettlement = 10006,
	}

	public class UIService : Service<UIService>
	{
		class UIShowInfo
		{
			public UIID id;
			public UI ui;
			public int layer;
		}
		// public Dictionary<int, string> uiPath = new Dictionary<int, string>();
		//public Dictionary<int, UI> uiList = new Dictionary<int, UI>();
		const int UI_LAYER_COUNT = 3;
		//List<UIShowInfo> showInfoList = new List<UIShowInfo>();
		Dictionary<int, UIShowInfo> showInfoDict = new Dictionary<int, UIShowInfo>();
		public Node2D[] layerRoot = new Node2D[UI_LAYER_COUNT];
		public override void Init()
		{
			for (int i = 0; i < UI_LAYER_COUNT; i++)
			{
				//uiLayers[i] = new List<UI>();
				layerRoot[i] = GameRoot.Instance.GetNode<Node2D>(string.Format("UILayers/{0}", i));
			}
		}
		public void Show(UIID uiID, object param = null)
		{
			var cfg = ConfigService.Instance.Get<Config.CfgUI>((int)uiID);
			if (null == cfg)
				return;
			if (showInfoDict.ContainsKey(cfg.id))
				return;
			string fullPath = string.Format("scenes/{0}.tscn", cfg.path);
			UI ui = Godot.GD.Load<PackedScene>(fullPath).Instance<UI>();
			layerRoot[cfg.layer].AddChild(ui);
			showInfoDict.Add(cfg.id, new UIShowInfo { layer = cfg.layer, id = uiID, ui = ui });
			ui.OnShow(param);
		}

		public void ShowOnly(UIID uiID, object param = null)
		{
			foreach (var id in (UIID[])Enum.GetValues(typeof(UIID)))
			{
				if (id == uiID)
				{
					if (!showInfoDict.ContainsKey((int)id))
						Show(id, param);
				}
				else
				{
					Close(id);
				}
			}
		}

		public void CloseAll()
		{
			foreach (var id in (UIID[])Enum.GetValues(typeof(UIID)))
				Close(id);
		}

		private void Destroy(UIShowInfo info)
		{
			info.ui.OnDestroy();
			layerRoot[info.layer].RemoveChild(info.ui);
			showInfoDict.Remove((int)info.id);
		}

		public void Destroy(UI ui)
		{
			foreach (var info in showInfoDict.Values)
				if (info.ui == ui)
				{
					Destroy(info);
					break;
				}
		}

		public void Close(UIID id)
		{
			if (showInfoDict.TryGetValue((int)id, out var info))
			{
				if (info.ui.OnClose())
					Destroy(info);
			}
		}

		// public void Close(UI ui)
		// {
		// 	foreach (var info in showInfoList)
		// 		if (info.ui == ui && info.ui.OnClose())
		// 			Destroy(info);
		// }
	}
}