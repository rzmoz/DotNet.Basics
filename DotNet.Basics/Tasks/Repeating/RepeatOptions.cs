using System;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tasks.Repeating
{
    public class RepeatOptions : TaskOptions
    {
        private uint? _maxTries;
        private TimeSpan? _timeout;

        public RepeatOptions()
        {
            RetryDelay = 250.MilliSeconds();
        }

        public TimeSpan RetryDelay { get; set; }

        public uint? MaxTries
        {
            get { return _maxTries; }
            set
            {
                _maxTries = value;
                CountLoopBreakPredicate = MaxTries == null ? null : new CountLoopBreakPredicate(MaxTries.Value);
            }
        }

        public TimeSpan? Timeout
        {
            get { return _timeout; }
            set
            {
                _timeout = value;
                TimeoutLoopBreakPredicate = Timeout == null ? null : new TimeoutLoopBreakPredicate(Timeout.Value);
            }
        }

        public Action Ping { get; set; }
        public Action Finally { get; set; }
        public Type IgnoreExceptionType { get; set; }

        internal CountLoopBreakPredicate CountLoopBreakPredicate { get; private set; }
        internal TimeoutLoopBreakPredicate TimeoutLoopBreakPredicate { get; private set; }
    }
}
