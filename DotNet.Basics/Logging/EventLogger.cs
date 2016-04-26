using System;
using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.Logging
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
