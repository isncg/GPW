using Godot;
using System.Collections.Generic;


namespace GPW
{
	public class BattleRoot : Node
	{
		public List<BulletManager> bulletManagers;
		public override void _Ready()
		{
			base._Ready();
		}

		public void Init()
		{
			bulletManagers = new List<BulletManager>();

			for (int i = 0; i < 64; i++)
			{
				var bm = GetNode<BulletManager>(string.Format("BulletManagerList/{0}", i));
				if (null == bm) break;
				bulletManagers.Add(bm);
			}
		}

		public void TestFire()
		{
			BulletSpawnParam param = new BulletSpawnParam();
			param.offset = new Vector2(100, 200);
			param.rotation = 0;
			param.delay = 0;
			param.speed = 100;
			var bstn = BulletSpawnTreeNode.Get(0);
			var paramList = bstn.BuildParamList(param);
			foreach (var p in paramList)
				BulletManager.CreateInstance(bulletManagers, p);
		}

		public override void _Input(InputEvent inputEvent)
		{
			// base._Input(inputEvent);
			// if (inputEvent is InputEventKey key)
			// {

			// }
			if (inputEvent is InputEventKey keyEvent && keyEvent.Pressed)
			{
				if (keyEvent.Scancode == (uint)KeyList.T)
					TestFire();
			}
		}

	}
}