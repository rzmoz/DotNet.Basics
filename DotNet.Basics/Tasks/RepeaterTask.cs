using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tasks
{
    public class RepeaterTask
    {
        private Type _ignoreExceptionsOfType;

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

            bool success;
            try//ensure finally is executed
            {
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
                        {
                            success = true;
                            break;
                        }

                    if (ShouldContinue(lastException) == false)
                    {
                        success = false;
                        break;
                    }

                    await Task.Delay(RetryDelay).ConfigureAwait(false);

                } while (true);
            }
            finally
            {
                try
                {
                    Finally?.Invoke();//we don't catch exceptions here since it needs to float if any
                }
                catch (Exception e)
                {
                    if (lastException == null)
                        throw e;
                    throw new AggregateException(lastException, e);
                }
            }

            return success;
        }

        private void Pingback()
        {
            if (Ping == null)
                return;

            try
            {
                Ping.Invoke();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
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
        internal Action Finally { get; set; }
        public TimeSpan RetryDelay { get; internal set; }


    }
}
