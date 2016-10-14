using System;

namespace DotNet.Basics.Tasks.Repeating
{
    public class RepeatTimeoutPredicate
    {

        private DateTime _startTime;

        public RepeatTimeoutPredicate(TimeSpan timeout)
        {
            Timeout = timeout;
            _startTime = DateTime.MaxValue;
            Init();
        }

        public TimeSpan Timeout { get; }

        public void Init()
        {
            _startTime = DateTime.UtcNow;
        }

        public bool ShouldBreak()
        {
            return DateTime.UtcNow - _startTime > Timeout;
        }

        public void LoopCallback()
        {
        }
    }
}
