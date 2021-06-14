using Godot;
using System;

namespace GPW
{
	public enum BulletDriverType
	{
		Direct = 0
	}

	namespace Config
	{
		public class CfgBulletDriver : Cfg
		{
			public BulletDriverType driverType = BulletDriverType.Direct;
			public float speedFactor = 1;
		}


	}

	public abstract class BulletDriver
	{
		public Config.CfgBulletDriver cfg = null;
		public float time = 0;
		public float duration = 30;

		public abstract void Init(Bullet node, BulletSpawnParam param);
		// public abstract void InitNode(Bullet node);
		// public abstract void InitParam(BulletSpawnParam spawnParam);
		public abstract void UpdateNode(Bullet node);
		public abstract void Release();

		public static BulletDriver Get(int id)
		{
			var cfg = ConfigService.Instance.Get<Config.CfgBulletDriver>(id);
			if (null == cfg) return null;
			BulletDriver result = null;
			switch (cfg.driverType)
			{
				case BulletDriverType.Direct:
					result = BulletDriverInPool<BulletDriver_Direct>.Alloc();
					break;
			}
			if (null != result)
				result.cfg = cfg;
			return result;
		}
	}
	public abstract class BulletDriverInPool<T> : BulletDriver where T : BulletDriverInPool<T>, new()
	{
		public static T Alloc() => Pool<T>.Alloc();
		public override void Release() { Pool<T>.Recycle((T)this); }
		public static Vector2 offscreenPos = new Vector2(0, 100000);
	}

	public class BulletDriver_Direct : BulletDriverInPool<BulletDriver_Direct>
	{

		public Vector2 origin = Vector2.Zero;
		public Vector2 direction = Vector2.Zero;

		public override void Init(Bullet node, BulletSpawnParam param)
		{
			time = -param.delay;
			node.SetOffsetScreen();

			origin.x = param.offset.x;
			origin.y = param.offset.y;

			float t = Mathf.Deg2Rad(param.rotation);
			direction.x = Mathf.Sin(t) * param.speed;
			direction.y = Mathf.Cos(t) * param.speed;
			node.Rotation = -t;
		}

		// public override void InitNode(Bullet node)
		// {
		// 	node.Rotation = (float)Math.Atan2(direction.y, direction.x);
		// 	node.Position = offscreenPos;
		// }

		// public override void InitParam(BulletSpawnParam spawnParam)
		// {
		// 	origin.x = spawnParam.offset.x;
		// 	origin.y = spawnParam.offset.y;

		// 	float t = Mathf.Deg2Rad(spawnParam.rotation);
		// 	direction.x = Mathf.Cos(t) * spawnParam.speed;
		// 	direction.y = Mathf.Sin(t) * spawnParam.speed;
		// }

		public override void UpdateNode(Bullet node)
		{
			if (time >= 0 && time < duration)
				node.Position = origin + direction * time;
		}
	}


}