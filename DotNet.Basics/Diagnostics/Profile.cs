using System;

namespace DotNet.Basics.Diagnostics
{
    public class Profile
    {
        public Profile(string name = null)
        {
            InternalStart = ActiveStart;
            InternalStop = Passive;
            Name = name ?? string.Empty;
            Duration = TimeSpan.Zero;
            InternalToString = () => $"'{Name}' not started";
        }

        public Profile(string name, DateTime startTime, DateTime endTime)
            : this(name)
        {
            StartTime = startTime;
            EndTime = endTime;
            Duration = EndTime - StartTime;
            InternalStart(startTime);
            InternalStop(endTime);
        }

        public string Name { get; set; }

        public DateTime StartTime { get; protected set; }
        public DateTime EndTime { get; protected set; }

        public TimeSpan Duration { get; protected set; }

        public bool Start()
        {
            return Start(DateTime.UtcNow);
        }
        public bool Start(DateTime utcNow)
        {
            return InternalStart(utcNow);
        }

        public bool Stop()
        {
            return Stop(DateTime.UtcNow);
        }
        public bool Stop(DateTime utcNow)
        {
            return InternalStop(utcNow);
        }

        private Func<DateTime, bool> InternalStart;
        private Func<DateTime, bool> InternalStop;
        private Func<string> InternalToString;

        private bool ActiveStart(DateTime start)
        {
            StartTime = start;
            InternalStart = Passive;
            InternalStop = ActiveStop;
            InternalToString = () => $"'{Name}' started";
            return true;
        }

        private bool ActiveStop(DateTime end)
        {
            EndTime = end;
            Duration = EndTime - StartTime;
            InternalStop = Passive;
            InternalToString = () => $"'{Name}' finished in {Duration}";
            return true;
        }

        private bool Passive(DateTime dt)
        {
            return false;
        }

        public override string ToString()
        {
            return InternalToString();
        }
    }
}
