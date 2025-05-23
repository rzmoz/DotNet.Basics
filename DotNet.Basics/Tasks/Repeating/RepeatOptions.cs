﻿using System;

namespace DotNet.Basics.Tasks.Repeating
{
    public sealed class RepeatOptions
    {
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMilliseconds(250);

        public int? MaxTries
        {
            get => RepeatMaxTriesPredicate?.MaxTries;
            set => RepeatMaxTriesPredicate = value == null ? null : new RepeatMaxTriesPredicate(value.Value);
        }

        public TimeSpan? Timeout
        {
            get => RepeatTimeoutPredicate?.Timeout;
            set => RepeatTimeoutPredicate = value == null ? null : new RepeatTimeoutPredicate(value.Value);
        }

        /// <summary>
        /// Will be invoked on every retry cycle
        /// </summary>
        public Action PingOnRetry { get; set; } = () => { };

        /// <summary>
        /// Exceptions of this type will be ignored and task will finish with success even if exceptions of this type occur
        /// </summary>
        public TypeList MuteExceptions { get; } = new(2);

        /// <summary>
        /// Will always be invoked once on finish regardless of result
        /// </summary>
        public Action Finally { get; set; } = () => { };

        internal RepeatMaxTriesPredicate? RepeatMaxTriesPredicate { get; private set; }
        internal RepeatTimeoutPredicate? RepeatTimeoutPredicate { get; private set; }
    }
}
