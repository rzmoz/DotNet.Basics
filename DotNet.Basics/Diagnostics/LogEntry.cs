using System;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public class LogEntry
    {
        public delegate void TaskLogEventHandler(LogEntry entry);

        public LogEntry(LogLevel level, string message, Exception exception = null)
        : this(DateTime.Now, level, message, exception)
        { }

        public LogEntry(DateTime timeStamp, LogLevel level, string message, Exception exception)
        {
            TimeStamp = timeStamp;
            Level = level;
            Message = message;
            Exception = exception;
        }

        public DateTime TimeStamp { get; }
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
            return $"{TimeStamp:s}:<{Level}> {Message} {exceptionString}";
        }
    }
}
