using System;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Diagnostics
{
    public class LogEntry : DiagnosticsEntry
    {
        public LogEntry(DateTime timestamp, string message, LogLevel level = LogLevel.Info, Exception exception = null)
            : base(timestamp, DiagnosticsType.Log)
        {
            Message = message;
            Level = level;
            Exception = exception;
            if (exception == null)
                return;
            Message = Message == null ? exception.ToString() : $"{Message}\r\n{Exception.ToString()}";
        }

        public string Message { get; set; }
        public LogLevel Level { get; }
        public Exception Exception { get; }

        protected override string GetMessage()
        {
            return $"<{Level.ToName()}> {Message}";
        }
    }
}
