using System;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tasks
{
    public sealed class RepeatOptions
    {
        private uint? _maxTries;
        private TimeSpan? _timeout;

        public RepeatOptions()
        {
            RetryDelay = 250.MilliSeconds();
            RunScope = RunScope.Transient;
            RunThread = RunThread.Current;
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

        public RunScope RunScope { get; set; }

        public RunThread RunThread { get; set; }

        /// <summary>
        /// Will be invoked on every retry cycle
        /// </summary>
        public Action PingOnRetry { get; set; }

        /// <summary>
        /// Exceptions of this type will be ignored and task will finish with success even if these exceptions occur
        /// </summary>
        public Type DontRethrowOnTaskFailedType { get; set; }

        /// <summary>
        /// will always be invoked once on finish regardless of result
        /// </summary>
        public Action Finally { get; set; }

        internal CountLoopBreakPredicate CountLoopBreakPredicate { get; private set; }
        internal TimeoutLoopBreakPredicate TimeoutLoopBreakPredicate { get; private set; }
    }
}
