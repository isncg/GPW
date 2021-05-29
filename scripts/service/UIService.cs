using System;
using System.Collections.Generic;
using Godot;
namespace GPW
{
	public enum UIType
	{
		UILaunching,
		UIMainMenu,
		UIBattleLoading,
		UIBattleStart,
		UIBattleFinish,
		UISettlement,
	}
	public class UIService : Service<UIService>
	{

		public Dictionary<int, string> uiPath = new Dictionary<int, string>();
		public Dictionary<int, UI> uiList = new Dictionary<int, UI>();
		public override void Init()
		{
			uiPath[(int)UIType.UILaunching] = "Launching";
			uiPath[(int)UIType.UIMainMenu] = "MainMenu";
		}

		public override void Reset()
		{
			uiPath.Clear();
		}


		public void Show(UIType uIType, object param = null)
		{
			if (uiPath.TryGetValue((int)uIType, out var path))
			{
				string fullPath = string.Format("scenes/{0}.tscn", path);
				UI ui = Godot.GD.Load<PackedScene>(fullPath).Instance<UI>();
				var gameRoot = GameRoot.Instance;
				gameRoot.UIroot.AddChild(ui);
				uiList[(int)uIType] = ui;
			}

		}

		public void Close(UIType uIType)
		{
			if (uiList.TryGetValue((int)uIType, out var ui))
			{
				GameRoot.Instance.UIroot.RemoveChild(ui);
			}
		}
	}
}