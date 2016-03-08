using System;
using System.Globalization;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Diagnostics
{
    public class ProfileEntry : DiagnosticsEntry
    {
        public ProfileEntry(DateTime timestamp, string name, TimeSpan duration, DurationFormattingUnit durationFormattingUnit = DurationFormattingUnit.Seconds)
            : base(timestamp, DiagnosticsType.Profile)
        {
            Name = name;
            Duration = duration;
            DurationFormattingUnit = durationFormattingUnit;
        }

        public string Name { get; set; }
        public TimeSpan Duration { get; }
        public DurationFormattingUnit DurationFormattingUnit { get; set; }

        protected override string GetMessage()
        {
            double value = 0.0;
            string unit = string.Empty;
            switch (DurationFormattingUnit)
            {
                case DurationFormattingUnit.MilliSeconds:
                    value = Duration.TotalMilliseconds;
                    unit = "milliseconds";
                    break;
                case DurationFormattingUnit.Seconds:
                    value = Duration.TotalSeconds;
                    unit = "seconds";
                    break;
                case DurationFormattingUnit.Minutes:
                    value = Duration.TotalMinutes;
                    unit = "minutes";
                    break;
                case DurationFormattingUnit.Hours:
                    value = Duration.TotalHours;
                    unit = "hours";
                    break;
                default:
                    throw new ArgumentException($"DurationFormattingUnit not supported: {DurationFormattingUnit.ToName()}");
            }

            return $"'{Name}' finished in {value.ToString("0.00", new CultureInfo("en-US"))} {unit}";
        }
    }
}
