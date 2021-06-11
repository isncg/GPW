using System;

namespace GPW
{
	public class FileService : Service<FileService>
	{
		static string CreateLogFileName()
		{
			return string.Format("{0}.log", DateTime.Now);
		}
		public string LogFilePath { get; private set; } = CreateLogFileName();
		public override void Init()
		{
			LogFilePath = CreateLogFileName();
		}
	}
}