using System;

namespace DotNet.Basics.Diagnostics
{
    public class Profiler
    {
        private readonly ProfilerFormatter _formatter = new ProfilerFormatter();

        public Profiler(string name = null)
            : this(name, DateTime.MinValue, DateTime.MinValue, isFinished: false)
        {
        }

        public Profiler(string name, DateTime startTime, DateTime endTime, bool isFinished = true)
        {
            if (startTime > endTime)
                throw new ArgumentException($"Start time has to be before end time. Start:{startTime} End:{endTime}");
            InternalStart = ActiveStart;
            InternalStop = Passive;
            Name = name ?? string.Empty;
            StartTime = startTime;
            EndTime = endTime;
            State = ProfilerStates.NotStarted;
            if (isFinished)
            {
                InternalStart(startTime);
                InternalStop(endTime);
            }
        }

        public string Name { get; set; }

        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public TimeSpan Duration => EndTime - StartTime;
        public ProfilerStates State { get; private set; }
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

        private bool ActiveStart(DateTime start)
        {
            StartTime = start;
            InternalStart = Passive;
            InternalStop = ActiveStop;
            State = ProfilerStates.Running;
            return true;
        }

        private bool ActiveStop(DateTime end)
        {
            EndTime = end;
            InternalStop = Passive;
            State = ProfilerStates.Finished;
            return true;
        }

        private bool Passive(DateTime dt)
        {
            return false;
        }

        public override string ToString()
        {
            return _formatter.Format(this);
        }
        public string ToString(DurationFormattingUnit durationFormattingUnit)
        {
            return _formatter.Format(this, durationFormattingUnit);
        }
    }
}
