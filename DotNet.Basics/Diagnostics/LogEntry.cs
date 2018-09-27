using System;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public class LogEntry
    {
        public LogEntry(LogLevel logLevel, string message, Exception exception = null)
        {
            LogLevel = logLevel;
            Message = message ?? string.Empty;
            Exception = exception;
            HasException = Exception != null;
        }

        public LogLevel LogLevel { get; }
        public string Message { get; }
        public Exception Exception { get; }

        public bool HasException { get; }

        public override string ToString()
        {
            return $"{Message}";
        }
    }
}
