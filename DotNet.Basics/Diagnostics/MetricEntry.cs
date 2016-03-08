using System;

namespace DotNet.Basics.Diagnostics
{
    public class MetricEntry : DiagnosticsEntry
    {
        public MetricEntry(DateTime timestamp, string name, double value = 0.0)
            : base(timestamp, DiagnosticsType.Metric)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public double Value { get; }

        protected override string GetMessage()
        {
            return $"'{Name}':{Value}";
        }
    }
}
