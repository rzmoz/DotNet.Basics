using System;
using System.Globalization;

namespace DotNet.Basics.Diagnostics
{
    public class ProfilerFormatter
    {
        public string Format(string name, ProfilerState state, TimeSpan duration)
        {
            return Format(name, state, () =>
            {
                var output = $"finished in {duration.ToString(string.Empty, new CultureInfo("en-US"))}";
                if (string.IsNullOrWhiteSpace(name) == false)
                    output = $"'{name}' {output}";
                return output;
            });
        }
        public string Format(string name, ProfilerState state, TimeSpan duration, DurationFormattingUnit formattingUnit, CultureInfo cultureInfo = null)
        {
            return Format(name, state, () =>
              {
                  double value;
                  switch (formattingUnit)
                  {
                      case DurationFormattingUnit.MilliSeconds:
                          value = duration.TotalMilliseconds;
                          break;
                      case DurationFormattingUnit.Seconds:
                          value = duration.TotalSeconds;
                          break;
                      case DurationFormattingUnit.Minutes:
                          value = duration.TotalMinutes;
                          break;
                      case DurationFormattingUnit.Hours:
                          value = duration.TotalHours;
                          break;
                      case DurationFormattingUnit.Days:
                          value = duration.TotalDays;
                          break;
                      default:
                          throw new ArgumentException($"DurationFormattingUnit not supported: {formattingUnit}");
                  }
                  if (cultureInfo == null)
                      cultureInfo = new CultureInfo("en-US");
                  var unit = Enum.GetName(typeof(DurationFormattingUnit), formattingUnit) ?? formattingUnit.ToString();
                  var output = $"finished in {value.ToString("0.00", cultureInfo)} {unit}";

                  if (string.IsNullOrWhiteSpace(name) == false)
                      output = $"'{name}' {output}";

                  return output;
              });
        }

        private string Format(string name, ProfilerState state, Func<string> formatDuration)
        {
            switch (state)
            {
                case ProfilerState.NotStarted:
                    return $"'{name}' not started";
                case ProfilerState.Running:
                    return $"'{name}' started";
                case ProfilerState.Finished:
                    return formatDuration();
                default:
                    throw new ArgumentException($"Profile state not supported: {state}");
            }
        }
    }
}
