using System;
using System.Globalization;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Diagnostics
{
    public class ProfilerFormatter
    {
        public string Format(Profiler profiler)
        {
            return Format(profiler, p =>
            {
                var output = $"finished in {profiler.Duration.ToString(string.Empty, new CultureInfo("en-US"))}";

                if (string.IsNullOrWhiteSpace(profiler.Name) == false)
                    output = $"'{profiler.Name}' {output}";

                return output;
            });
        }
        public string Format(Profiler profiler, DurationFormattingUnit formattingUnit)
        {
            return Format(profiler, p =>
            {
                double value;
                string unit;
                switch (formattingUnit)
                {
                    case DurationFormattingUnit.MilliSeconds:
                        value = profiler.Duration.TotalMilliseconds;
                        unit = "milliseconds";
                        break;
                    case DurationFormattingUnit.Seconds:
                        value = profiler.Duration.TotalSeconds;
                        unit = "seconds";
                        break;
                    case DurationFormattingUnit.Minutes:
                        value = profiler.Duration.TotalMinutes;
                        unit = "minutes";
                        break;
                    case DurationFormattingUnit.Hours:
                        value = profiler.Duration.TotalHours;
                        unit = "hours";
                        break;
                    case DurationFormattingUnit.Days:
                        value = profiler.Duration.TotalDays;
                        unit = "days";
                        break;
                    default:
                        throw new ArgumentException($"DurationFormattingUnit not supported: {formattingUnit.ToName()}");
                }

                var output = $"finished in {value.ToString("0.00", new CultureInfo("en-US"))} {unit}";

                if (string.IsNullOrWhiteSpace(profiler.Name) == false)
                    output = $"'{profiler.Name}' {output}";

                return output;
            });
        }

        private string Format(Profiler profiler, Func<Profiler, string> formatting)
        {
            if (profiler == null) throw new ArgumentNullException(nameof(profiler));
            switch (profiler.State)
            {
                case ProfilerStates.NotStarted:
                    return $"'{profiler.Name}' not started";
                case ProfilerStates.Running:
                    return $"'{profiler.Name}' started";
                case ProfilerStates.Finished:
                    return formatting(profiler);
                default:
                    throw new ArgumentException($"Profile state not supported: {profiler.State}");
            }
        }
    }
}
