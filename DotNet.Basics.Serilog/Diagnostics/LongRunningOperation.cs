using System;

namespace DotNet.Basics.Serilog.Diagnostics
{
    public class LongRunningOperation(string name, string? id = null)
    {
        public string Name { get; } = name;
        public string Id { get; } = id ?? Guid.CreateVersion7(DateTimeOffset.UtcNow).ToString();
        public DateTime StartTime { get; } = DateTime.Now;
        public TimeSpan DurationNow => DateTime.Now - StartTime;
        public string DurationNowFormatted => $@"{DurationNow:hh\:mm\:ss}";
    }
}
