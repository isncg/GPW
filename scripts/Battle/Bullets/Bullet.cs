using Godot;
using System;

namespace GPW
{
	public class Bullet : Node2D
	{
		public BulletDriver driver;

		public override void _Process(float delta)
		{
			base._Process(delta);
			if (null != driver)
			{
				driver.time += delta;
				driver.UpdateNode(this);
			}
			//Log.I("[Bullet:Process] {0}_{1}", this.Position, this.GetHashCode());
		}
	}
}