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
		PackedScene templete;
		//Bullet templete;
		// Called when the node enters the scene tree for the first time.
		public int MaxQueueSize = 2048;
		public Queue<Bullet> instanceQueue = new Queue<Bullet>();
		public Queue<Bullet> allocationQueue = new Queue<Bullet>();
		public override void _Ready()
		{
			//templete = GetNode<Bullet>("templete");
		}

		public void CreateInstance<T>(Action<T> driverInit) where T : BulletDriverInPool<T>, new()
		{
			var inst = allocationQueue.Count > 0 ? allocationQueue.Dequeue() : templete.Instance<Bullet>();
			if (allocationQueue.Count > 0)
			{
				var driver = BulletDriverInPool<T>.Alloc();
				driverInit(driver);
				driver.InitNode(inst);
				inst.driver = driver;
				instanceQueue.Enqueue(inst);
			}
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
			foreach (var inst in instanceQueue)
			{
				inst.driver.duration += delta;
				inst.driver.UpdateNode(inst);
			}
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