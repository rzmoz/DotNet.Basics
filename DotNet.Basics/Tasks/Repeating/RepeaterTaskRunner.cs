using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks.Repeating
{
    public class RepeaterTaskRunner
    {
        public async Task<bool> RunAsync(ManagedTask<EventArgs> task, Func<Exception, Task<bool>> untilPredicate, RepeatOptions options = null)
        {
            if (task == null)
                return false;

            if (untilPredicate == null)
                throw new ArgumentNullException(nameof(untilPredicate), $"Task will potentially run forever. Set untilPredicate and also consider adding timeout and maxtries to task options");

            if (options == null)
                options = new RepeatOptions();

            Exception lastException = null;
            options.RepeatMaxTriesPredicate?.Init();
            options.RepeatTimeoutPredicate?.Init();

            bool success;

            try//ensure finally is executed
            {
                do
                {
                    Exception exceptionInLastLoop = null;
                    try
                    {
                        await task.RunAsync(EventArgs.Empty, CancellationToken.None).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        lastException = e;
                        exceptionInLastLoop = e;

                    }
                    finally
                    {
                        options.RepeatMaxTriesPredicate?.LoopCallback();
                    }

                    try
                    {
                        if (await untilPredicate.Invoke(exceptionInLastLoop).ConfigureAwait(false))
                        {
                            success = true;
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        lastException = e;
                        if (IgnoreException(e, options.MuteExceptions) == false)
                            throw;
                    }

                    if (ShouldContinue(lastException, options) == false)
                    {
                        success = false;
                        break;
                    }

                    RetryPingBack(options);

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

        private void RetryPingBack(RepeatOptions options)
        {
            options.PingOnRetry?.Invoke();
        }


        private bool ShouldContinue(Exception lastException, RepeatOptions options)
        {
            bool breakPrematurely = options.RepeatMaxTriesPredicate != null && options.RepeatMaxTriesPredicate.ShouldBreak() ||
                                    options.RepeatTimeoutPredicate != null && options.RepeatTimeoutPredicate.ShouldBreak();

            if (breakPrematurely)
            {
                if (lastException == null)
                    return false;

                if (options.MuteExceptions == null || options.MuteExceptions.Count == 0)
                    throw lastException;

                if (IgnoreException(lastException, options.MuteExceptions))
                    return false;

                throw lastException;
            }
            return true;
        }

        private bool IgnoreException(Exception e, IList<Type> ignoredExceptions)
        {
            var lastExceptionType = e.GetType();

            return ignoredExceptions.Contains(lastExceptionType) ||
                    ignoredExceptions.Any(mutedException => lastExceptionType.IsSubclassOf(mutedException));
        }
    }
}
