using System;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public class Log
    {
        public delegate void MessageLoggedEventHandler(LogLevel level, string message, Exception e);
        public static event MessageLoggedEventHandler MessageLogged;

        static Log()
        {
            Logger = new Log();
        }

        public static Log Logger { get; }

        public static void Verbose(string message, Exception e = null)
        {
            MessageLogged?.Invoke(LogLevel.Trace, message, e);
        }
        public static void Debug(string message, Exception e = null)
        {
            MessageLogged?.Invoke(LogLevel.Debug, message, e);
        }
        public static void Information(string message, Exception e = null)
        {
            MessageLogged?.Invoke(LogLevel.Information, message, e);
        }
        public static void Warning(string message, Exception e = null)
        {
            MessageLogged?.Invoke(LogLevel.Warning, message, e);
        }
        public static void Error(string message, Exception e = null)
        {
            MessageLogged?.Invoke(LogLevel.Error, message, e);
        }
        public static void Critical(string message, Exception e = null)
        {
            MessageLogged?.Invoke(LogLevel.Critical, message, e);
        }
    }
}
