using System;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public class RawLogEntry
    {
        public RawLogEntry(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            LogLevel = logLevel;
            EventId = eventId;
            State = state;
            Exception = exception;
            Formatter = formatter;
        }

        public LogLevel LogLevel { get; }
        public int EventId { get; }
        public object State { get; }
        public Exception Exception { get; }
        public Func<object, Exception, string> Formatter { get; }
    }
}
