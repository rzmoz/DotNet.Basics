using System;
using System.Diagnostics;

namespace DotNet.Basics.Tasks
{
    public class TimeoutLoopBreakPredicate
    {
        private readonly TimeSpan _timeout;
        private readonly Stopwatch _stopWatch;

        public TimeoutLoopBreakPredicate(TimeSpan timeout)
        {
            _timeout = timeout;
            _stopWatch = new Stopwatch();
            Reset();
        }

        public void Reset()
        {
            _stopWatch.Restart();
        }

        public bool ShouldBreak()
        {
            return _stopWatch.Elapsed > _timeout;
        }
    }
}
