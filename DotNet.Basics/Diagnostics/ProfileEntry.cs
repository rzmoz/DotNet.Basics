using System;

namespace DotNet.Basics.Diagnostics
{
    public class ProfileEntry
    {
        private readonly ProfileFormatter _formatter = new ProfileFormatter();

        public ProfileEntry(DateTime timestamp, string name, TimeSpan duration)
        {
            Timestamp = timestamp;
            Name = name;
            Duration = duration;
        }

        public DateTime Timestamp { get; }
        public string Name { get; }
        public TimeSpan Duration { get; }

        public string ToString(DurationFormattingUnit unit)
        {
            var startTime = DateTime.MinValue;
            var endtime = startTime.Add(Duration);
            return _formatter.Format(new Profile(Name, startTime, endtime), unit);
        }
        public override string ToString()
        {
            var startTime = DateTime.MinValue;
            var endtime = startTime.Add(Duration);
            return _formatter.Format(new Profile(Name, startTime, endtime));
        }
    }
}
