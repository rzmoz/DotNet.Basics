using System;

namespace DotNet.Basics.Diagnostics
{
    public class EventLogger : DotNetBasicsLogger
    {
        public event EventHandler<LogEntry> EntryLogged;

        protected override void Log(LogEntry entry)
        {
            EntryLogged?.Invoke(null, entry);
        }
    }
}
