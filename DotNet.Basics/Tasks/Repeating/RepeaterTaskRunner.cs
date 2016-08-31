using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks.Repeating
{
    public class RepeaterTaskRunner
    {
        public async Task<bool> RunAsync(RunTask<RepeatOptions> task, Func<Exception, bool> untilPredicate)
        {
            if (task == null)
                return false;

            if (untilPredicate == null)
                throw new ArgumentNullException(nameof(untilPredicate), $"Task will potentially run forever. Set untilPredicate and also consider adding timeout and maxtries to task options");

            Exception lastException = null;
            task.Options.CountLoopBreakPredicate?.Reset();
            task.Options.TimeoutLoopBreakPredicate?.Reset();

            bool success;
            try//ensure finally is executed
            {
                do
                {
                    Exception exceptionInLastLoop;
                    try
                    {
                        await task.RunAsync().ConfigureAwait(false);
                        task.Options.CountLoopBreakPredicate?.LoopCallback();
                        exceptionInLastLoop = null;
                    }
                    catch (Exception e)
                    {
                        lastException = e;
                        exceptionInLastLoop = e;
                        task.Options.CountLoopBreakPredicate?.LoopCallback();
                    }

                    Pingback(task.Options);

                    if (untilPredicate.Invoke(exceptionInLastLoop))
                    {
                        success = true;
                        break;
                    }

                    if (ShouldContinue(lastException, task.Options) == false)
                    {
                        success = false;
                        break;
                    }

                    await Task.Delay(task.Options.RetryDelay).ConfigureAwait(false);

                } while (true);
            }
            finally
            {
                try
                {
                    task.Options.Finally?.Invoke();//we don't catch exceptions here since it needs to float if any
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

        private void Pingback(RepeatOptions options)
        {
            if (options.Ping == null)
                return;

            try
            {
                options.Ping.Invoke();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        private bool ShouldContinue(Exception lastException, RepeatOptions options)
        {
            bool breakPrematurely = options.CountLoopBreakPredicate != null && options.CountLoopBreakPredicate.ShouldBreak() ||
                               options.TimeoutLoopBreakPredicate != null && options.TimeoutLoopBreakPredicate.ShouldBreak();

            if (breakPrematurely)
            {
                if (lastException == null)
                    return false;

                if (options.IgnoreExceptionType == null)
                    throw lastException;

                if (lastException.GetType().IsSubclassOf(options.IgnoreExceptionType) ||
                    lastException.GetType() == options.IgnoreExceptionType)
                    return false;

                throw lastException;
            }
            return true;
        }
    }
}
