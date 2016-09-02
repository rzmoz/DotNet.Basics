using System;
using System.Globalization;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Diagnostics
{
    public class ProfileFormatter
    {
        public string Format(Profile profile)
        {
            return Format(profile, p =>
            {
                var output = $"finished in {profile.Duration.ToString(string.Empty, new CultureInfo("en-US"))}";

                if (string.IsNullOrWhiteSpace(profile.Name) == false)
                    output = $"'{profile.Name}' {output}";

                return output;
            });
        }
        public string Format(Profile profile, DurationFormattingUnit formattingUnit)
        {
            return Format(profile, p =>
            {
                double value;
                string unit;
                switch (formattingUnit)
                {
                    case DurationFormattingUnit.MilliSeconds:
                        value = profile.Duration.TotalMilliseconds;
                        unit = "milliseconds";
                        break;
                    case DurationFormattingUnit.Seconds:
                        value = profile.Duration.TotalSeconds;
                        unit = "seconds";
                        break;
                    case DurationFormattingUnit.Minutes:
                        value = profile.Duration.TotalMinutes;
                        unit = "minutes";
                        break;
                    case DurationFormattingUnit.Hours:
                        value = profile.Duration.TotalHours;
                        unit = "hours";
                        break;
                    case DurationFormattingUnit.Days:
                        value = profile.Duration.TotalDays;
                        unit = "days";
                        break;
                    default:
                        throw new ArgumentException($"DurationFormattingUnit not supported: {formattingUnit.ToName()}");
                }

                var output = $"finished in {value.ToString("0.00", new CultureInfo("en-US"))} {unit}";

                if (string.IsNullOrWhiteSpace(profile.Name) == false)
                    output = $"'{profile.Name}' {output}";

                return output;
            });
        }

        private string Format(Profile profile, Func<Profile, string> formatting)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            switch (profile.State)
            {
                case ProfileStates.NotStarted:
                    return $"'{profile.Name}' not started";
                case ProfileStates.Running:
                    return $"'{profile.Name}' started";
                case ProfileStates.Finished:
                    return formatting(profile);
                default:
                    throw new ArgumentException($"Profile state not supported: {profile.State}");
            }
        }
    }
}
