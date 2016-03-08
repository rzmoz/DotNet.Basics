using System;

namespace DotNet.Basics.Diagnostics
{
    public class EventDiagnostics : IDiagnostics
    {
        public event EventHandler<DiagnosticsEntry> EntryLogged;

        public void Log(string message, LogLevel logLevel = LogLevel.Info, Exception exception = null)
        {
            EntryLogged?.Invoke(null, new LogEntry(GetTimestamp(), message, logLevel, exception));
        }

        public void Metric(string name, double value)
        {
            EntryLogged?.Invoke(null, new MetricEntry(GetTimestamp(), name, value));
        }

        public void Metric(string name, int value)
        {
            EntryLogged?.Invoke(null, new MetricEntry(GetTimestamp(), name, value));
        }

        public void Profile(string taskName, TimeSpan duration)
        {
            EntryLogged?.Invoke(null, new ProfileEntry(GetTimestamp(), taskName, duration));
        }

        protected virtual DateTime GetTimestamp()
        {
            return DateTime.UtcNow;
        }
    }
}
