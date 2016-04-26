using System;

namespace DotNet.Basics.Diagnostics
{
    public class MetricEntry
    {
        public MetricEntry(DateTime timestamp, string name, double value = 0.0)
        {
            Timestamp = timestamp;
            Name = name;
            Value = value;
        }

        public DateTime Timestamp { get; set; }
        public string Name { get; set; }
        public double Value { get; }
    }
}
