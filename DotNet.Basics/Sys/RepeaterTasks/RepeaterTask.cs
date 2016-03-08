using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CSharp.Basics.Sys.RepeaterTasks
{
    public abstract class RepeaterTask
    {
        private Type _ignoreExceptionsOfType = null;

        protected RepeaterTask(Action action)
            : this()
        {
            Action = () => Task.Factory.StartNew(action);
        }

        protected RepeaterTask(Func<Task> action)
            : this()
        {
            Action = action;
        }

        private RepeaterTask()
        {
            RetryDelay = 500.MilliSeconds();
        }

        protected async Task<bool> RunAsync()
        {
            if (UntilPredicate == null)
                throw new NoStopConditionIsSetException("Task will potentially run forever. Set Until and consider adding WithTimeout and/or WithMaxTries");

            Exception lastException = null;
            CountLoopBreakPredicate?.Reset();
            TimeoutLoopBreakPredicate?.Reset();

            do
            {
                Exception exceptionInLastLoop;
                try
                {
                    await Action.Invoke().ConfigureAwait(false);
                    CountLoopBreakPredicate?.LoopCallback();
                    exceptionInLastLoop = null;
                }
                catch (Exception e)
                {
                    lastException = e;
                    exceptionInLastLoop = e;
                    CountLoopBreakPredicate?.LoopCallback();
                }

                Pingback();

                if (UntilPredicate != null)
                    if (UntilPredicate.Invoke(exceptionInLastLoop))
                        return true;

                if (ShouldContinue(lastException) == false)
                    return false;

                await Task.Delay(RetryDelay).ConfigureAwait(false);

            } while (true);
        }

        private void Pingback()
        {
            if (Ping != null)
            {
                try
                {
                    Ping.Invoke();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
        }

        private bool ShouldContinue(Exception lastException)
        {
            bool shouldBreak = CountLoopBreakPredicate != null && CountLoopBreakPredicate.ShouldBreak() ||
                               TimeoutLoopBreakPredicate != null && TimeoutLoopBreakPredicate.ShouldBreak();

            if (shouldBreak)
            {
                if (lastException == null)
                    return false;

                if (_ignoreExceptionsOfType == null)
                    throw lastException;

                if (lastException.GetType().IsSubclassOf(_ignoreExceptionsOfType) ||
                    lastException.GetType() == _ignoreExceptionsOfType)
                    return false;

                throw lastException;
            }
            return true;
        }

        internal void InteralIgnoreExceptionsOfType(Type exceptionTypeToIgnore)
        {
            _ignoreExceptionsOfType = exceptionTypeToIgnore;
        }

        private Func<Task> Action { get; set; }
        internal CountLoopBreakPredicate CountLoopBreakPredicate { get; set; }
        internal TimeoutLoopBreakPredicate TimeoutLoopBreakPredicate { get; set; }
        internal Func<Exception, bool> UntilPredicate { get; set; }
        internal Action Ping { get; set; }
        public TimeSpan RetryDelay { get; internal set; }

    }
}
