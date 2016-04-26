using System;

namespace DotNet.Basics.Diagnostics
{
    public class EventDiagnostics : IDiagnostics
    {
        public event EventHandler<MetricEntry> MetricLogged;
        public event EventHandler<ProfileEntry> ProfileLogged;
        
        public void Metric(string name, double value)
        {
            MetricLogged?.Invoke(null, new MetricEntry(GetTimestamp(), name, value));
        }

        public void Metric(string name, int value)
        {
            MetricLogged?.Invoke(null, new MetricEntry(GetTimestamp(), name, value));
        }

        public void Profile(string taskName, TimeSpan duration)
        {
            ProfileLogged?.Invoke(null, new ProfileEntry(GetTimestamp(), taskName, duration));
        }

        protected virtual DateTime GetTimestamp()
        {
            return DateTime.UtcNow;
        }
    }
}
