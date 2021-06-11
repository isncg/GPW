using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;
namespace GPW
{

	public class ConfigService : Service<ConfigService>
	{
		public override void Init()
		{
			base.Init();
			InitTable<Config.CfgString>();
			InitTable<Config.CfgUI>();
			InitTable<Config.CfgServer>();
		}

		public override void Reset()
		{
			base.Reset();
		}

		private Dictionary<Type, Dictionary<int, Config.Cfg>> tables = new Dictionary<Type, Dictionary<int, Config.Cfg>>();
		public T Get<T>(int id) where T : Config.Cfg
		{
			if (tables.TryGetValue(typeof(T), out var t))
				if (t.TryGetValue(id, out var cfg))
					return (T)cfg;
			return null;
		}

		public void Enumrate<T>(Action<T> action) where T : Config.Cfg
		{
			if (tables.TryGetValue(typeof(T), out var t))
				foreach (var cfg in t.Values)
					action((T)cfg);
		}

		public T Find<T>(Predicate<T> match) where T : Config.Cfg
		{
			if (tables.TryGetValue(typeof(T), out var t))
			{
				foreach (var cfg in t.Values)
				{
					var cfgT = (T)cfg;
					if (match(cfgT))
						return cfgT;
				}
			}
			return null;
		}

		private void InitTable<T>() where T : Config.Cfg, new()
		{
			string filePath = string.Format("./cfg/{0}.json", typeof(T).Name);
			if (File.Exists(filePath))
			{
				var deserialized = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(filePath));
				Dictionary<int, Config.Cfg> table = new Dictionary<int, Config.Cfg>();
				foreach (var cfg in deserialized)
					table[cfg.id] = cfg;
				tables[typeof(T)] = table;
			}
			else
			{
				var list = new List<T>();
				list.Add(new T());
				string serialized = JsonConvert.SerializeObject(list);
				var file = new System.IO.FileInfo(filePath);
				file.Directory.Create();
				File.WriteAllText(file.FullName, serialized);
			}
		}
	}

	namespace Config
	{
		public class Cfg
		{
			public int id;
		}
		public class CfgString : Cfg
		{
			public string dev;
			public string cn;
			public string en;
			public string jp;
		}

		public class CfgUI : Cfg
		{
			public string path;
			public int layer;
		}

		public class CfgServer : Cfg
		{
			public string name;
			public string host;
			public int port;
		}
	}
}