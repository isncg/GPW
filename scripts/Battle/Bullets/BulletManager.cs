using Godot;
using System;
using System.Collections.Generic;

namespace GPW
{
	public class BulletManager : Node2D
	{
		// Declare member variables here. Examples:
		// private int a = 2;
		// private string b = "text";

		[Export]
		PackedScene templete = default;
		//Bullet templete;
		// Called when the node enters the scene tree for the first time.
		public int MaxQueueSize = 2048;
		public HashSet<Bullet> instanceSet = new HashSet<Bullet>();
		public Queue<Bullet> allocationQueue = new Queue<Bullet>();

		Bullet CreateBullet()
		{
			// 4.0 没有TryDequeue
			Bullet result = null;
			if (allocationQueue.Count > 0)
			{
				result = allocationQueue.Dequeue();
			}
			if (null == result)
			{
				result = templete.Instance<Bullet>();
			}
			if (null != result)
			{
				this.AddChild(result);
				instanceSet.Add(result);
				result.manager = this;
			}
			return result;

		}

		public static void CreateInstance(List<BulletManager> managers, BulletSpawnParam param)
		{
			var bulletCfg = ConfigService.Instance.Get<Config.CfgBullet>(param.bulletID);
			var mgr = managers[bulletCfg.layer];
			var driver = BulletDriver.Get(bulletCfg.driver);
			var bulletNode = mgr.CreateBullet();
			driver.Init(bulletNode, param);
			bulletNode.driver = driver;
		}


		public void Recycle(Bullet inst)
		{
			inst.SetOffsetScreen();
			if (!instanceSet.Contains(inst))
				return;
			instanceSet.Remove(inst);
			if (null != inst.driver)
			{
				inst.driver.Release();
				inst.driver = null;
			}
			this.RemoveChild(inst);
			allocationQueue.Enqueue(inst);
		}
	}
}