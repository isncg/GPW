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
		public Queue<Bullet> instanceQueue = new Queue<Bullet>();
		public Queue<Bullet> allocationQueue = new Queue<Bullet>();
		public override void _Ready()
		{
			//templete = GetNode<Bullet>("templete");
		}

		// public void CreateInstance<T>(BulletSpawnParam param) where T : BulletDriverInPool<T>, new()
		// {
		// 	var inst = allocationQueue.Count > 0 ? allocationQueue.Dequeue() : templete.Instance<Bullet>();
		// 	if (allocationQueue.Count > 0)
		// 	{
		// 		var driver = BulletDriverInPool<T>.Alloc();
		// 		//driverInit(driver);
		// 		driver.InitNode(inst);
		// 		driver.InitParam(param);
		// 		inst.driver = driver;
		// 		instanceQueue.Enqueue(inst);
		// 	}
		// }

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
				this.AddChild(result);
			}
			if (null != result)
			{
				instanceQueue.Enqueue(result);
			}
			return result;

		}

		// public void CreateInstance(BulletSpawnParam param, BulletDriver driver)// where T : BulletDriverInPool<T>, new()
		// {
		// 	var inst = allocationQueue.Count > 0 ? allocationQueue.Dequeue() : templete.Instance<Bullet>();
		// 	if (allocationQueue.Count > 0)
		// 	{
		// 		//var driver = BulletDriverInPool<T>.Alloc();
		// 		//driverInit(driver);
		// 		driver.InitNode(inst);
		// 		driver.InitParam(param);
		// 		inst.driver = driver;
		// 		instanceQueue.Enqueue(inst);
		// 	}
		// }

		public static void CreateInstance(List<BulletManager> managers, BulletSpawnParam param)
		{
			var bulletCfg = ConfigService.Instance.Get<Config.CfgBullet>(param.bulletID);
			var mgr = managers[bulletCfg.layer];
			var driver = BulletDriver.Get(bulletCfg.driver);
			var bulletNode = mgr.CreateBullet();
			driver.InitNode(bulletNode);
			driver.InitParam(param);
			bulletNode.driver = driver;
		}


		private void Release(Bullet inst)
		{
			var driver = inst.driver;
			inst.driver = null;
			allocationQueue.Enqueue(inst);
			driver.Release();
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(float delta)
		{
			while (instanceQueue.Count > MaxQueueSize)
			{
				var inst = instanceQueue.Dequeue();
				Release(inst);
			}
			do
			{
				if (instanceQueue.Count <= 0) break;
				var driver = instanceQueue.Peek().driver;
				if (driver.time > driver.duration)
				{
					var inst = instanceQueue.Dequeue();
					Release(inst);
				}
				else
				{
					break;
				}
			} while (true);
		}
	}


}