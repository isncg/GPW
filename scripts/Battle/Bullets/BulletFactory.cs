using System.Collections.Generic;
using Godot;

namespace GPW
{
	public class BulletSpawnParam
	{
		public Vector2 offset = Vector2.Zero;
		public float rotation = 0;
		public float delay = 0;
		public float speed = 0;

		public BulletSpawnParam() { }
		public BulletSpawnParam(BulletSpawnParam other)
		{
			offset = other.offset * 1;
			rotation = other.rotation;
			delay = other.delay;
			speed = other.delay;
		}
	}

	public abstract class BulletDistribution
	{
		public abstract void Get(int index, int total, BulletSpawnParam input, BulletSpawnParam output);

		public static BulletDistribution Get(int id)
		{
			var param = ConfigService.Instance.Get<Config.CfgBulletDistributionParam>(id);
			if (null == param)
				return null;
			switch (param.paramType)
			{
				case BulletDistributionType.LinearSpace:
					return new LinearSpaceBulletDistribution { configID = id };
				case BulletDistributionType.StepArcSpace:
					return new StepArcSpaceBulletDistribution { configID = id };
				case BulletDistributionType.CircleSpace:
					return new CircleSpaceBulletDistribution { configID = id };
				case BulletDistributionType.LinearTime:
					return new LinearTimeBulletDistribution { configID = id };
			}
			return null;
		}
	}

	public abstract class SerializeableBulletDistribution<T> : BulletDistribution where T : Config.Cfg
	{
		public int configID;
		public T GetConfig() => ConfigService.Instance.Get<T>(configID);
	}

	public enum BulletDistributionType
	{
		LinearSpace,  // begin, end
		StepArcSpace, // radiance, gap
		CircleSpace,  // radiance
		LinearTime    // interval
	}

	namespace Config
	{
		public class CfgBulletDistributionParam : Cfg
		{
			public BulletDistributionType paramType;
			public Vector2 begin;
			public Vector2 end;
			public float radiance;
			public float gap;
			public float interval;
		}
	}
	public class LinearSpaceBulletDistribution : SerializeableBulletDistribution<Config.CfgBulletDistributionParam>
	{
		static Vector2 tmpLerp = new Vector2();
		public override void Get(int index, int total, BulletSpawnParam input, BulletSpawnParam output)
		{
			var cfg = GetConfig();
			float weight = 0;
			if (total > 1)
				weight = (float)index / (total - 1);
			float t = Mathf.Deg2Rad(input.rotation);
			tmpLerp.x = Mathf.Lerp(cfg.begin.x, cfg.end.x, weight);
			tmpLerp.y = Mathf.Lerp(cfg.begin.y, cfg.end.y, weight);
			output.offset.x = Mathf.Cos(t) * tmpLerp.x - Mathf.Sin(tmpLerp.y) + input.offset.x;
			output.offset.y = Mathf.Sin(t) * tmpLerp.x + Mathf.Cos(tmpLerp.y) + input.offset.y;
		}
	}

	public class StepArcSpaceBulletDistribution : SerializeableBulletDistribution<Config.CfgBulletDistributionParam>
	{
		// public float radiance;
		// public float step;
		public override void Get(int index, int total, BulletSpawnParam input, BulletSpawnParam output)
		{
			//throw new System.NotImplementedException();
			// [0] 
			// [-.5, .5]
			// [-1,0,1]
			// [-1.5, -0.5, 0.5, 1.5]
			var cfg = GetConfig();
			float rotation = (-0.5f * (total - 1) + index) * cfg.gap + input.rotation;
			float t = Mathf.Deg2Rad(rotation);
			output.offset.x = Mathf.Cos(t) * cfg.radiance + input.offset.x;
			output.offset.y = Mathf.Sin(t) * cfg.radiance + input.offset.y;
		}
	}

	public class CircleSpaceBulletDistribution : SerializeableBulletDistribution<Config.CfgBulletDistributionParam>
	{
		//public float radiance;
		public override void Get(int index, int total, BulletSpawnParam input, BulletSpawnParam output)
		{
			var cfg = GetConfig();
			float rotation = index * 360.0f / total + input.rotation;
			float t = Mathf.Deg2Rad(rotation);
			output.offset.x = Mathf.Cos(t) * cfg.radiance + input.offset.x;
			output.offset.y = Mathf.Sin(t) * cfg.radiance + input.offset.y;
		}
	}

	public class LinearTimeBulletDistribution : SerializeableBulletDistribution<Config.CfgBulletDistributionParam>
	{
		//public float interval;
		public override void Get(int index, int total, BulletSpawnParam input, BulletSpawnParam output)
		{
			float delay = (-0.5f * (total - 1) + index) * GetConfig().interval + input.delay;
			output.delay = input.delay + delay;
		}
	}

	namespace Config
	{
		public class CfgBulletSpawnTreeNode : Cfg
		{
			public List<int> childrenIDs;
			public List<int> distributionIDs;
			public int degree;
		}
	}
	public class BulletSpawnTreeNode
	{
		//public BulletSpawnTreeNode next;
		public List<BulletSpawnTreeNode> children = null;
		List<BulletDistribution> combinedDistribution = new List<BulletDistribution>();
		public int degree;
		public List<BulletSpawnParam> BuildParamList(BulletSpawnParam param)
		{
			List<BulletSpawnParam> curLevelParams = new List<BulletSpawnParam>(degree);
			for (int i = 0; i < degree; i++)
			{
				curLevelParams[i] = new BulletSpawnParam(param);
				foreach (var dist in combinedDistribution)
					dist.Get(i, degree, param, curLevelParams[i]);
			}

			if (null == children)
				return curLevelParams;

			List<BulletSpawnParam> childrenResults = new List<BulletSpawnParam>();
			for (int i = 0; i < degree; i++)
			{
				if (i < children.Count && null != children[i])
					childrenResults.AddRange(children[i].BuildParamList(curLevelParams[i]));
				else
					childrenResults.Add(curLevelParams[i]);
			}

			return childrenResults;
		}

		public static BulletSpawnTreeNode Get(int id, int searchDepth = 0)
		{
			if (id <= 0 || searchDepth > 10) return null;
			Config.CfgBulletSpawnTreeNode cfg = ConfigService.Instance.Get<Config.CfgBulletSpawnTreeNode>(id);
			if (null == cfg) return null;
			BulletSpawnTreeNode result = new BulletSpawnTreeNode();
			if (cfg.childrenIDs != null && cfg.childrenIDs.Count > 0)
			{
				result.children = new List<BulletSpawnTreeNode>();
				foreach (var childID in cfg.childrenIDs)
				{
					result.children.Add(Get(childID, searchDepth + 1));
				}
			}
			else
			{
				result.children = null;
			}

			result.combinedDistribution.Clear();
			foreach (var distributionID in cfg.distributionIDs)
			{
				result.combinedDistribution.Add(BulletDistribution.Get(distributionID));
			}

			return result;
		}
	}

	// public abstract class BulletFactory
	// {
	// 	public BulletFactory nextLevelFactory = null;
	// 	public abstract void Create(BulletFactoryCreationParam param, ref List<BulletDriver> output);
	// }
	// public class DirectBulletFactory : BulletFactory
	// {
	// 	public float speed = 1000;
	// 	public override void Create(BulletFactoryCreationParam param, ref List<BulletDriver> output)
	// 	{
	// 		var driver = Pool<BulletDriver_Direct>.Alloc();
	// 		driver.origin = param.offset;
	// 		driver.direction = new Vector2(Mathf.Cos(Mathf.Deg2Rad(param.rotation)), (Mathf.Sin(Mathf.Deg2Rad(param.rotation)))) * speed;
	// 		output.Add(driver);
	// 	}
	// }

	// public class LinearOffsetBulletFactory : BulletFactory
	// {
	// 	public Vector2 beginOffset;
	// 	public Vector2 endOffset;
	// 	public int count;

	// 	public override void Create(BulletFactoryCreationParam param, ref List<BulletDriver> output)
	// 	{
	// 		throw new System.NotImplementedException();
	// 	}
	// }

	// public class SprialOffsetBulletFactory : BulletFactory
	// {
	// 	public float radiance;
	// 	public float rotationBegin;
	// 	public float rotationEnd;
	// 	public int count;
	// 	public override void Create(BulletFactoryCreationParam param, ref List<BulletDriver> output)
	// 	{
	// 		throw new System.NotImplementedException();
	// 	}
	// }

	// public class RotationBulletFactory : BulletFactory
	// {
	// 	public float rotationBegin;
	// 	public float rotationEnd;
	// 	public int count;
	// 	public override void Create(BulletFactoryCreationParam param, ref List<BulletDriver> output)
	// 	{
	// 		if (null == nextLevelFactory) return;
	// 		for (int i = 0; i < count; i++)
	// 		{
	// 			float weight = 0;
	// 			if (count > 1)
	// 				weight = (float)i / (count - 1);
	// 			var iterParam = new BulletFactoryCreationParam();
	// 			iterParam.rotation = Mathf.Lerp(rotationBegin, rotationEnd, weight) + param.rotation;
	// 			iterParam.offset = param.offset;
	// 			iterParam.delay = param.delay;
	// 			nextLevelFactory.Create(iterParam, ref output);
	// 		}
	// 	}
	// }

	// public class DelayBulletFactory : BulletFactory
	// {
	// 	public float delayBegin;
	// 	public float delayEnd;
	// 	public int count;
	// 	public override void Create(BulletFactoryCreationParam param, ref List<BulletDriver> output)
	// 	{
	// 		throw new System.NotImplementedException();
	// 	}
	// }

	// public class LinearBulletFactory : BulletFactory
	// {
	// 	public Vector2 beginPos;
	// 	public Vector2 endPos;
	// 	public float beginTime;
	// 	public float endTime;
	// 	public bool rotateToParamDirection = false;//TBD
	// 	int count;

	// 	public override void Create(BulletFactoryCreationParam param, ref List<BulletDriver> output)
	// 	{
	// 		if (null == nextLevelFactory || count <= 0) return;
	// 		for (int i = 0; i < count; i++)
	// 		{
	// 			BulletFactoryCreationParam iterParam = new BulletFactoryCreationParam();
	// 			float weight = 0;
	// 			if (count > 1)
	// 				weight = (float)i / (count - 1);
	// 			iterParam.offset = new Vector2(
	// 				Mathf.Lerp(beginPos.x, endPos.x, weight),
	// 				Mathf.Lerp(beginPos.y, endPos.y, weight)
	// 				) + param.offset;
	// 			iterParam.delay = Mathf.Lerp(beginTime, endTime, weight) + param.delay;
	// 			iterParam.rotation = param.rotation;
	// 			nextLevelFactory.Create(iterParam, ref output);
	// 		}

	// 		//throw new System.NotImplementedException();
	// 	}
	// }
}