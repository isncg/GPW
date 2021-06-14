using Godot;
using System;

namespace GPW
{
	public class Bullet : Node2D
	{
		public static Vector2 offscreenPos = new Vector2(0, 100000);
		public static readonly float MaxDuration = 2;
		public BulletDriver driver;
		public BulletManager manager;

		public void SetOffsetScreen()
		{
			this.Position = offscreenPos;
		}
		public override void _Process(float delta)
		{
			base._Process(delta);
			if (null != driver)
			{
				driver.time += delta;
				if (driver.time > MaxDuration)
					manager.Recycle(this);
				else
					driver.UpdateNode(this);
			}
			//Log.I("[Bullet:Process] {0}_{1}", this.Position, this.GetHashCode());
		}
	}
}