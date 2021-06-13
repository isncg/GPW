using System;
using System.IO;

namespace GPW
{
	public static class Log
	{
		static string CreateLogFileName()
		{
			var now = DateTime.Now;
			Directory.CreateDirectory("log");
			return string.Format("log/{0}_{1}_{2} {3}_{4}_{5}.log", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
		}
		public static readonly string LogFilePath = CreateLogFileName();
		public enum LogLevel
		{
			Info = 0,
			Warning = 1,
			Error = 2,
		}
		public static LogLevel logLevel = LogLevel.Info;
		static void WriteLog(string level, string content, bool toConsole)
		{
			var now = DateTime.Now;

			string line = string.Format("[{0}/{1}/{2} {3}:{4}:{5}:{6}][{7}]{8}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond, level, content);
			File.AppendAllLines(LogFilePath, new string[] { line });
			//File.AppendAllLines("logtest.log", new string[] { line });
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