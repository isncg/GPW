using System;
using System.IO;

namespace GPW
{
	public class FileService : Service<FileService>
	{
		static string CreateLogFileName()
		{
			var now = DateTime.Now;
			Directory.CreateDirectory("log");
			return string.Format("log/{0}_{1}_{2} {3}_{4}_{5}.log", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
			//return now.ToString() + ".log";
		}
		public string LogFilePath { get; private set; } = CreateLogFileName();
		public override void Init()
		{
			LogFilePath = CreateLogFileName();
		}
	}
}