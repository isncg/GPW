using System;
using System.IO;

namespace GPW
{
	public static class Log
	{
		public enum LogLevel
		{
			Info = 0,
			Warning = 1,
			Error = 2,
		}
		public static LogLevel logLevel = LogLevel.Info;
		static void WriteLog(string level, string content, bool toConsole)
		{
			string line = string.Format("[{0}][{1}]{2}", DateTime.Now, level, content);
			File.AppendAllLines(FileService.Instance.LogFilePath, new string[] { line });
			if (toConsole)
				Console.WriteLine(line);
		}

		public static void I(string log) => WriteLog("I", log, logLevel <= LogLevel.Info);
		public static void W(string log) => WriteLog("W", log, logLevel <= LogLevel.Warning);
		public static void E(string log) => WriteLog("E", log, logLevel <= LogLevel.Error);

		public static void I(string log, params object[] args) => WriteLog("I", string.Format(log, args), logLevel <= LogLevel.Info);
		public static void W(string log, params object[] args) => WriteLog("W", string.Format(log, args), logLevel <= LogLevel.Warning);
		public static void E(string log, params object[] args) => WriteLog("E", string.Format(log, args), logLevel <= LogLevel.Error);
	}
}