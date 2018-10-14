using System;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public class LogEntry
    {
        public delegate void TaskLogEventHandler(LogEntry entry);

        public LogEntry(LogLevel level, string message, Exception exception = null)
        {
            Level = level;
            Message = message ?? string.Empty;
            Exception = exception;
        }

        public LogLevel Level { get; }
        public string Message { get; private set; }
        public Exception Exception { get; }

        public void AddMessagePrefix(string prefix)
        {
            Message = prefix + Message;
        }

        public override string ToString()
        {
            var exceptionString = Exception == null ? string.Empty : $" | {Exception}";
            return $"{Message}{exceptionString}";
        }
    }
}
