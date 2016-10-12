using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class RepeaterTaskRunner
    {

        public async Task<bool> RunAsync(ManagedTask task, Func<Exception, bool> untilPredicate, RepeatOptions options = null)
        {
            if (task == null)
                return false;

            if (untilPredicate == null)
                throw new ArgumentNullException(nameof(untilPredicate), $"Task will potentially run forever. Set untilPredicate and also consider adding timeout and maxtries to task options");

            if (options == null)
                options = new RepeatOptions();

            Exception lastException = null;
            options.CountLoopBreakPredicate?.Reset();
            options.TimeoutLoopBreakPredicate?.Reset();

            bool success;

            try//ensure finally is executed
            {
                do
                {
                    Exception exceptionInLastLoop = null;
                    try
                    {
                        await task.RunAsync("").ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        lastException = e;
                        exceptionInLastLoop = e;

                    }
                    finally
                    {
                        options.CountLoopBreakPredicate?.LoopCallback();
                    }

                    if (untilPredicate.Invoke(exceptionInLastLoop))
                    {
                        success = true;
                        break;
                    }

                    if (ShouldContinue(lastException, options) == false)
                    {
                        success = false;
                        break;
                    }

                    RetryPingback(options);

                    await Task.Delay(options.RetryDelay).ConfigureAwait(false);

                } while (true);
            }
            finally
            {
                try
                {
                    options.Finally?.Invoke();
                }
                catch (Exception e)
                {
                    if (lastException == null)
                        throw;
                    throw new AggregateException(lastException, e);
                }
            }


            return success;
        }

        private void RetryPingback(RepeatOptions options)
        {
            if (options.PingOnRetry == null)
                return;

            try
            {
                options.PingOnRetry();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
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

                if (options.DontRethrowOnTaskFailedType == null)
                    throw lastException;

                if (lastException.GetType().IsSubclassOf(options.DontRethrowOnTaskFailedType) ||
                    lastException.GetType() == options.DontRethrowOnTaskFailedType)
                    return false;

                throw lastException;
            }
            return true;
        }
    }
}
