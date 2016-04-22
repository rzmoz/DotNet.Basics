using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tasks
{
    public class RepeaterTask
    {
        public RepeaterTask(Action action)
        {
            Action = () => Task.Run(action);
        }
        public RepeaterTask(Func<Task> action)
        {
            Action = action;
        }

        public RepeaterTask WithNoRetryDelay()
        {
            RetryDelay = 5.MilliSeconds();
            return this;
        }

        public RepeaterTask WithRetryDelay(TimeSpan retryDelay)
        {
            RetryDelay = retryDelay;
            return this;
        }

        public RepeaterTask IgnoreExceptionsOfType(Type exceptionTypeToIgnore)
        {
            IgnoreExceptionType = exceptionTypeToIgnore;
            return this;
        }

        public RepeaterTask WithPing(Action pingAction)
        {
            Ping = pingAction;
            return this;
        }

        public RepeaterTask WithTimeout(TimeSpan timeout)
        {
            TimeoutLoopBreakPredicate = new TimeoutLoopBreakPredicate(timeout);
            return this;
        }
        public RepeaterTask WithMaxTries(uint maxTries)
        {
            CountLoopBreakPredicate = new CountLoopBreakPredicate(maxTries);
            return this;
        }

        public RepeaterTask WithFinally(Action finallyAction)
        {
            Finally = finallyAction;
            return this;
        }

        public RepeaterTask Until(Func<bool> untilPredicate)
        {
            UntilPredicate = e => untilPredicate.Invoke();
            return this;
        }

        public RepeaterTask UntilNoExceptions()
        {
            UntilPredicate = e => e == null;
            return this;
        }

        public async Task<bool> NowAsync()
        {
            try
            {
                var runner = new RepeaterTaskRunner();
                return await runner.RunAsync(this).ConfigureAwait(false);
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException;
            }
        }

        public bool Now()
        {
            try
            {
                var runner = new RepeaterTaskRunner();
                var task = runner.RunAsync(this);
                task.Wait();
                return task.Result;
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException;
            }
        }
        internal Func<Task> Action { get; private set; }
        internal CountLoopBreakPredicate CountLoopBreakPredicate { get; private set; }
        internal TimeoutLoopBreakPredicate TimeoutLoopBreakPredicate { get; private set; }
        internal Func<Exception, bool> UntilPredicate { get; private set; }
        internal Action Ping { get; private set; }
        internal Action Finally { get; private set; }
        internal Type IgnoreExceptionType { get; private set; }
        public TimeSpan RetryDelay { get; private set; }
    }
}