using System;
using System.Text.RegularExpressions;

namespace DotNet.Basics.Sys
{
    public static class TimeSpanExtensions
    {
        private const string _timeSpanPattern = @"^(\d+)([s|m|h|d|t]|ms)$";
        private static readonly Regex _timeSpanRegex = new(_timeSpanPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private const string _formatExceptionText = @"Input must be in format {time}{unit} where time is an integer and unit is ms|s|m|h|d|t. Was: ";

        /// <summary>
        /// input must be in format [time][unit] where time is an integer and unit is 
        /// ms = milliseconds
        /// s = seconds
        /// m = minutes
        /// h = hour
        /// t = ticks
        /// unit is case-insensitive
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static TimeSpan ToTimeSpan(this string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            input = input.Trim();

            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("input is empty");

            var match = _timeSpanRegex.Match(input);
            if (!match.Success || match.Groups.Count != 3)
                throw new FormatException(_formatExceptionText + input);

            var number = int.Parse(match.Groups[1].Value);
            var unit = match.Groups[2].Value.ToLower();

            switch (unit)
            {
                case "ms":
                    return number.MilliSeconds();
                case "s":
                    return number.Seconds();
                case "m":
                    return number.Minutes();
                case "h":
                    return number.Hours();
                case "d":
                    return number.Days();
                case "t":
                    return number.Ticks();
                default:
                    throw new FormatException(_formatExceptionText + input);
            }
        }

        public static TimeSpan MilliSeconds(this int number)
        {
            return TimeSpan.FromMilliseconds(number);
        }
        public static TimeSpan MilliSeconds(this uint number)
        {
            return TimeSpan.FromMilliseconds(number);
        }
        public static TimeSpan Seconds(this int number)
        {
            return TimeSpan.FromSeconds(number);
        }
        public static TimeSpan Seconds(this uint number)
        {
            return TimeSpan.FromSeconds(number);
        }
        public static TimeSpan Minutes(this int number)
        {
            return TimeSpan.FromMinutes(number);
        }
        public static TimeSpan Minutes(this uint number)
        {
            return TimeSpan.FromMinutes(number);
        }
        public static TimeSpan Hours(this int number)
        {
            return TimeSpan.FromHours(number);
        }
        public static TimeSpan Hours(this uint number)
        {
            return TimeSpan.FromHours(number);
        }
        public static TimeSpan Days(this int number)
        {
            return TimeSpan.FromDays(number);
        }
        public static TimeSpan Days(this uint number)
        {
            return TimeSpan.FromDays(number);
        }
        public static TimeSpan Ticks(this int number)
        {
            return TimeSpan.FromTicks(number);
        }
        public static TimeSpan Ticks(this uint number)
        {
            return TimeSpan.FromTicks(number);
        }
    }
}
