using System;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public static class Log
    {
        static Log()
        {
            Logger = new LogDispatcher();
        }

        public static LogDispatcher Logger { get; }

        public static void CloseAndFlush()
        {
            Logger.CloseAndFlush();
        }

        public static void Verbose(string message)
        {
            Verbose(message, null);
        }
        public static void Verbose(string message, Exception e)
        {
            Logger.Verbose(message, e);
        }
        public static void Debug(string message)
        {
            Debug(message, null);
        }
        public static void Debug(string message, Exception e)
        {
            Logger.Debug(message, e);
        }
        public static void Information(string message)
        {
            Information(message, null);
        }
        public static void Information(string message, Exception e)
        {
            Logger.Information(message, e);
        }
        public static void Warning(string message)
        {
            Warning(message, null);
        }
        public static void Warning(string message, Exception e)
        {
            Logger.Warning(message, e);
        }
        public static void Error(string message)
        {
            Error(message, null);
        }
        public static void Error(string message, Exception e)
        {
            Logger.Error(message, e);
        }
        public static void Critical(string message)
        {
            Critical(message, null);
        }
        public static void Critical(string message, Exception e)
        {
            Logger.Critical(message, e);
        }
        public static void Write(LogLevel level, string message)
        {
            Write(level, message, null);
        }
        public static void Write(LogLevel level, string message, Exception e)
        {
            Logger.Write(level, message, e);
        }
    }
}
