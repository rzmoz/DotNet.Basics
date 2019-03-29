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

        public static void Verbose(string message)
        {
            Verbose(message, null);
        }
        public static void Verbose(string message, Exception e)
        {
            MessageLogged?.Invoke(LogLevel.Trace, message, e);
        }
        public static void Debug(string message)
        {
            Debug(message, null);
        }
        public static void Debug(string message, Exception e)
        {
            MessageLogged?.Invoke(LogLevel.Debug, message, e);
        }
        public static void Information(string message)
        {
            Information(message, null);
        }
        public static void Information(string message, Exception e)
        {
            MessageLogged?.Invoke(LogLevel.Information, message, e);
        }
        public static void Warning(string message)
        {
            Warning(message, null);
        }
        public static void Warning(string message, Exception e)
        {
            MessageLogged?.Invoke(LogLevel.Warning, message, e);
        }
        public static void Error(string message)
        {
            Error(message, null);
        }
        public static void Error(string message, Exception e)
        {
            MessageLogged?.Invoke(LogLevel.Error, message, e);
        }
        public static void Critical(string message)
        {
            Critical(message, null);
        }
        public static void Critical(string message, Exception e)
        {
            MessageLogged?.Invoke(LogLevel.Critical, message, e);
        }
    }
}
