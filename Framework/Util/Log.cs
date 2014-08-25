using System;

namespace Framework
{
	public class Log
	{
		public static void Print(object msg)
		{
			Console.WriteLine(msg.ToString());
		}

		public static void Debug(object msg)
		{
			Print("D: " + msg.ToString());
		}

		public static void Info(object msg)
		{
			Print("I: " + msg.ToString());
		}

		public static void Warn(object msg)
		{
			Print("W: " + msg.ToString());
		}

		public static void Error(object msg)
		{
			Print("E: " + msg.ToString());
		}
	}
}

