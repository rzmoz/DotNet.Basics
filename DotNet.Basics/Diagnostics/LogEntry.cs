using System;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public class LogEntry
    {
        public LogEntry(DateTime timestamp, string message, LogLevel level, Exception exception = null)
        {
            Timestamp = timestamp;
            Message = message;
            Level = level;
            Exception = exception;
        }

        public DateTime Timestamp { get; }
        public string Message { get; }
        public LogLevel Level { get; }
        public Exception Exception { get; }

        public override string ToString()
        {
            return Message;
        }
    }
}
