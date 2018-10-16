using System;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public class LoggingContext : IHasLogging
    {
        public event LogEntry.TaskLogEventHandler EntryLogged;

        public LoggingContext(string messagePrefix = null)
        {
            MessagePrefix = messagePrefix;
        }

        public string MessagePrefix { get; }

        public void LogTrace(string message, Exception e = null)
        {
            Log(LogLevel.Trace, message, e);
        }
        public void LogDebug(string message, Exception e = null)
        {
            Log(LogLevel.Debug, message, e);
        }
        public void LogInformation(string message, Exception e = null)
        {
            Log(LogLevel.Information, message, e);
        }
        public void LogWarning(string message, Exception e = null)
        {
            Log(LogLevel.Warning, message, e);
        }
        public void LogError(string message, Exception e = null)
        {
            Log(LogLevel.Error, message, e);
        }
        public void LogCritical(string message, Exception e = null)
        {
            Log(LogLevel.Critical, message, e);
        }

        public void Log(LogLevel level, string message, Exception e = null)
        {
            Log(new LogEntry(level, message, e));
        }
        public void Log(LogEntry entry)
        {
            if (entry == null)
                return;
            if (MessagePrefix != null)
                entry.AddMessagePrefix($"{MessagePrefix} ");
            EntryLogged?.Invoke(entry);
        }
    }
}
