using System;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public class LogEntry
    {
        public LogEntry(LogLevel level, string message, Exception exception = null)
        {
            Level = level;
            Message = message ?? string.Empty;
            Exception = exception;
        }

        public LogLevel Level { get; }
        public string Message { get; }
        public Exception Exception { get; }

        public override string ToString()
        {
            return $"{Message} | {Exception}";
        }
    }
}
