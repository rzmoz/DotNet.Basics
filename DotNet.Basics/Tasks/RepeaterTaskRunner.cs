using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class RepeaterTaskRunner
    {
        public async Task<bool> RunAsync(TaskVessel<RepeatOptions> vessel, Func<Exception, bool> untilPredicate)
        {
            if (vessel == null)
                return false;

            if (untilPredicate == null)
                throw new ArgumentNullException(nameof(untilPredicate), $"Task will potentially run forever. Set untilPredicate and also consider adding timeout and maxtries to task options");

            Exception lastException = null;
            vessel.Options.CountLoopBreakPredicate?.Reset();
            vessel.Options.TimeoutLoopBreakPredicate?.Reset();

            bool success;
            try//ensure finally is executed
            {
                do
                {
                    Exception exceptionInLastLoop;
                    try
                    {
                        await vessel.Task.RunAsync().ConfigureAwait(false);
                        vessel.Options.CountLoopBreakPredicate?.LoopCallback();
                        exceptionInLastLoop = null;
                    }
                    catch (Exception e)
                    {
                        lastException = e;
                        exceptionInLastLoop = e;
                        vessel.Options.CountLoopBreakPredicate?.LoopCallback();
                    }

                    Pingback(vessel.Options);

                    if (untilPredicate.Invoke(exceptionInLastLoop))
                    {
                        success = true;
                        break;
                    }

                    if (ShouldContinue(lastException, vessel.Options) == false)
                    {
                        success = false;
                        break;
                    }

                    await Task.Delay(vessel.Options.RetryDelay).ConfigureAwait(false);

                } while (true);
            }
            finally
            {
                try
                {
                    vessel.Options.Finally?.Invoke();
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
