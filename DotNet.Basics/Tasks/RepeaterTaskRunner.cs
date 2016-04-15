using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class RepeaterTaskRunner
    {
        public async Task<bool> RunAsync(RepeaterTask task)
        {
            if (task == null)
                return false;

            if (task.UntilPredicate == null)
                throw new NoStopConditionIsSetException("Task will potentially run forever. Set Until and consider adding WithTimeout and/or WithMaxTries");

            Exception lastException = null;
            task.CountLoopBreakPredicate?.Reset();
            task.TimeoutLoopBreakPredicate?.Reset();

            bool success;
            try//ensure finally is executed
            {
                do
                {
                    Exception exceptionInLastLoop;
                    try
                    {
                        await task.Action.Invoke().ConfigureAwait(false);
                        task.CountLoopBreakPredicate?.LoopCallback();
                        exceptionInLastLoop = null;
                    }
                    catch (Exception e)
                    {
                        lastException = e;
                        exceptionInLastLoop = e;
                        task.CountLoopBreakPredicate?.LoopCallback();
                    }

                    Pingback(task);

                    if (task.UntilPredicate != null)
                        if (task.UntilPredicate.Invoke(exceptionInLastLoop))
                        {
                            success = true;
                            break;
                        }

                    if (ShouldContinue(lastException, task) == false)
                    {
                        success = false;
                        break;
                    }

                    await Task.Delay(task.RetryDelay).ConfigureAwait(false);

                } while (true);
            }
            finally
            {
                try
                {
                    task.Finally?.Invoke();//we don't catch exceptions here since it needs to float if any
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

        private void Pingback(RepeaterTask task)
        {
            if (task.Ping == null)
                return;

            try
            {
                task.Ping.Invoke();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        private bool ShouldContinue(Exception lastException, RepeaterTask task)
        {
            bool shouldBreak = task.CountLoopBreakPredicate != null && task.CountLoopBreakPredicate.ShouldBreak() ||
                               task.TimeoutLoopBreakPredicate != null && task.TimeoutLoopBreakPredicate.ShouldBreak();

            if (shouldBreak)
            {
                if (lastException == null)
                    return false;

                if (task.IgnoreExceptionType == null)
                    throw lastException;

                if (lastException.GetType().IsSubclassOf(task.IgnoreExceptionType) ||
                    lastException.GetType() == task.IgnoreExceptionType)
                    return false;

                throw lastException;
            }
            return true;
        }
    }
}
