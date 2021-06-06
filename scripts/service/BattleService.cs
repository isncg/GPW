using Godot;
using System.Collections.Generic;

namespace GPW
{
	public class BattleService : Service<BattleService>
	{
		public BattleRoot battleRoot;
		//public List<BulletManager> bulletManagers = new List<BulletManager>();
		public override void Init()
		{
			base.Init();
		}

		public void InitBattleRoot()
		{
			battleRoot = GameRoot.Instance.battleRootPacked.Instance<BattleRoot>();
			GameRoot.Instance.GetNode<Node>("BattleRoot").AddChild(battleRoot);
			battleRoot.Init();
		}

		public void ReleaseBattleRoot()
		{
			GameRoot.Instance.GetNode<Node>("BattleRoot").RemoveChild(battleRoot);
			battleRoot = null;
		}
	}
}