﻿using System;

namespace DotNet.Basics.Serilog
{
    public static class TimeSpanExtensions
    {
        public static string ToHumanReadableString(this TimeSpan t)
        {
            if (t <= TimeSpan.MinValue)
                return string.Empty;
            if (t.TotalSeconds <= 1)
                return $@"{t:s\.ff} seconds";
            if (t.TotalMinutes <= 1)
                return $@"{t:%s} seconds";
            if (t.TotalHours <= 1)
                return $@"{t:%m} minutes";
            if (t.TotalDays <= 1)
                return $@"{t:%h} hours";
            return $@"{t:%d} days";
        }
    }
}
