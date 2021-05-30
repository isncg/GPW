namespace GPW
{
	public static class Localization
	{
		public enum Region
		{
			Dev,
			CN,
			EN,
			JP
		}

		public static void SetRegion(Region region)
		{
			switch (region)
			{
				case Region.Dev:
					localizationFunc = DEV;
					break;
				case Region.CN:
					localizationFunc = CN;
					break;
				case Region.EN:
					localizationFunc = EN;
					break;
				case Region.JP:
					localizationFunc = JP;
					break;
			}
		}
		private static string DEV(Config.CfgString cfgString) => cfgString.dev;
		private static string CN(Config.CfgString cfgString) => cfgString.cn;
		private static string EN(Config.CfgString cfgString) => cfgString.en;
		private static string JP(Config.CfgString cfgString) => cfgString.jp;

		delegate string LocalizationFunc(Config.CfgString cfgString);
		private static LocalizationFunc localizationFunc = DEV;
		public static string Get(int id)
		{
			var cfg = ConfigService.Instance.Get<Config.CfgString>(id);
			if (null == cfg)
				return id.ToString();
			var result = localizationFunc(cfg);
			if (null == result)
				return id.ToString();
			return result;
		}
	}
}