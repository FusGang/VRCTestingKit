using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RuntimeUnityEditor.Core;

using VRCModLoader;

namespace VRCTestingKit
{
    public static class LoggerKit
    {
        internal readonly static List<string> Logs = new List<string>();

        public static void Log(string text)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            if (oldColor == ConsoleColor.Black)
                oldColor = ConsoleColor.Gray;

            if (Logs.Count > 254)
                Logs.RemoveAt(0);
            Logs.Add("<color=grey>[" + DateTime.Now.ToString("HH:mm:ss") + "][LOG] " + text + "</color>");

            if (Console.CursorLeft > 1)
                Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            VRCModLogger.Log("[VRCTestingKit] " + text);
            Console.ForegroundColor = oldColor;
        }
        public static void Log(object obj)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            if (oldColor == ConsoleColor.Black)
                oldColor = ConsoleColor.Gray;

            if (Logs.Count > 254)
                Logs.RemoveAt(0);
            Logs.Add("<color=grey>[" + DateTime.Now.ToString("HH:mm:ss") + "][LOG] " + obj.ToString() + "</color>");

            if (Console.CursorLeft > 1)
                Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            VRCModLogger.Log("[VRCTestingKit] " + obj.ToString());
            Console.ForegroundColor = oldColor;
        }

        public static void LogWarning(string text)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            if (oldColor == ConsoleColor.Black)
                oldColor = ConsoleColor.Gray;

            if (Logs.Count > 254)
                Logs.RemoveAt(0);
            Logs.Add("<color=orange>[" + DateTime.Now.ToString("HH:mm:ss") + "][WARN] " + text + "</color>");

            if (Console.CursorLeft > 1)
                Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            VRCModLogger.Log("[VRCTestingKit] " + text);
            Console.ForegroundColor = oldColor;
        }
        public static void LogWarning(object obj)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            if (oldColor == ConsoleColor.Black)
                oldColor = ConsoleColor.Gray;

            if (Logs.Count > 254)
                Logs.RemoveAt(0);
            Logs.Add("<color=orange>[" + DateTime.Now.ToString("HH:mm:ss") + "][WARN] " + obj.ToString() + "</color>");

            if (Console.CursorLeft > 1)
                Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            VRCModLogger.Log("[VRCTestingKit] " + obj.ToString());
            Console.ForegroundColor = oldColor;
        }

        public static void LogError(string text, Exception exception = null)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            if (oldColor == ConsoleColor.Black)
                oldColor = ConsoleColor.Gray;

            if (Logs.Count > 254)
                Logs.RemoveAt(0);
            Logs.Add("<color=red>[" + DateTime.Now.ToString("HH:mm:ss") + "][ERROR] " + text + "</color>");

            if (Console.CursorLeft > 1)
                Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            VRCModLogger.LogError("[VRCTestingKit] " + text);
            if (exception != null)
                VRCModLogger.LogError(exception.ToString());
            Console.ForegroundColor = oldColor;
        }
        public static void LogError(object obj, Exception exception = null)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            if (oldColor == ConsoleColor.Black)
                oldColor = ConsoleColor.Gray;

            if (Logs.Count > 254)
                Logs.RemoveAt(0);
            Logs.Add("<color=red>[" + DateTime.Now.ToString("HH:mm:ss") + "][ERROR] " + obj.ToString() + "</color>");

            if (Console.CursorLeft > 1)
                Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            VRCModLogger.LogError("[VRCTestingKit] " + obj.ToString());
            if (exception != null)
                VRCModLogger.LogError(exception.ToString());
            Console.ForegroundColor = oldColor;
        }

        public static void LogMessage(string text)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            if (oldColor == ConsoleColor.Black)
                oldColor = ConsoleColor.Gray;

            if (Logs.Count > 254)
                Logs.RemoveAt(0);
            Logs.Add("<color=green>[" + DateTime.Now.ToString("HH:mm:ss") + "][MSG] " + text + "</color>");

            if (Console.CursorLeft > 1)
                Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            VRCModLogger.Log("[VRCTestingKit] " + text);
            Console.ForegroundColor = oldColor;
        }
        public static void LogMessage(object obj)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            if (oldColor == ConsoleColor.Black)
                oldColor = ConsoleColor.Gray;

            if (Logs.Count > 254)
                Logs.RemoveAt(0);
            Logs.Add("<color=green>[" + DateTime.Now.ToString("HH:mm:ss") + "][MSG] " + obj.ToString() + "</color>");

            if (Console.CursorLeft > 1)
                Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            VRCModLogger.Log("[VRCTestingKit] " + obj.ToString());
            Console.ForegroundColor = oldColor;
        }
    }
    internal class RUELogger : ILoggerWrapper
    {
        public void Log(LogLevel logLogLevel, object content)
        {
            if (content.GetType() == typeof(string))
                content = ((string)content).Replace("[CheatTools]", "[RuntimeUnityEditor]");
            switch(logLogLevel)
            {
                case LogLevel.Error | LogLevel.Fatal:
                    LoggerKit.LogError(content);
                    break;
                case LogLevel.Warning:
                    LoggerKit.LogWarning(content);
                    break;
                case LogLevel.Message | LogLevel.Info:
                    LoggerKit.LogMessage(content);
                    break;
                default:
                    LoggerKit.Log(content);
                    break;
            }
        }
    }
}
