using System;

namespace DotNet.Basics.Diagnostics
{
    public class LongRunningOperation
    {
        public LongRunningOperation(string name, string id = null)
        {
            Name = name;
            Id = id ?? Guid.NewGuid().ToString();
            StartTime = DateTime.Now;
        }

        public string Name { get; }
        public string Id { get; }
        public DateTime StartTime { get; }
        public TimeSpan DurationNow => DateTime.Now - StartTime;
        public string DurationNowFormatted => $"{DurationNow:hh\\:mm\\:ss}";
    }
}
