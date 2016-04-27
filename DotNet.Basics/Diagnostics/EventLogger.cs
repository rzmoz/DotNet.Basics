using System;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public class EventLogger : DotNetBasicsLogger
    {
        public event EventHandler<LogEntry> EntryLogged;
        public event EventHandler<RawLogEntry> RawEntryLogged;

        public override void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            base.Log(logLevel, eventId, state, exception, formatter);
            RawEntryLogged?.Invoke(null, new RawLogEntry(logLevel, eventId, state, exception, formatter));
        }

        protected override void Log(LogEntry entry)
        {
            EntryLogged?.Invoke(null, entry);
        }
    }
}
