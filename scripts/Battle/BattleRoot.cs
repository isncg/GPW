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
				var bm = GetNode<BulletManager>(string.Format("BulletManager/{0}", i));
				if (null == bm) break;
				bulletManagers.Add(bm);
			}
		}

	}
}