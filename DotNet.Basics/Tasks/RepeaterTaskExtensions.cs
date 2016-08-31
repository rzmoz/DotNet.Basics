using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public static class RepeaterTaskExtensions
    {
        public static async Task<bool> UntilNoExceptionsAsync(this RepeaterTask task)
        {
            return await task.UntilAsync(e => e == null).ConfigureAwait(false);
        }

        public static async Task<bool> UntilAsync(this RepeaterTask task, Func<bool> untilPredicate)
        {
            return await task.UntilAsync(e => untilPredicate()).ConfigureAwait(false);
        }
        private static async Task<bool> UntilAsync(this RepeaterTask task, Func<Exception, bool> untilPredicate)
        {
            try
            {
                var runner = new RepeaterTaskRunner();
                return await runner.RunAsync(task, untilPredicate).ConfigureAwait(false);
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException;
            }
        }

        public static bool UntilNoExceptions(this RepeaterTask task)
        {
            return task.Until(e => e == null);
        }

        public static bool Until(this RepeaterTask task, Func<bool> untilPredicate)
        {
            return task.Until(e => untilPredicate());
        }

        private static bool Until(this RepeaterTask task, Func<Exception, bool> untilPredicate)
        {
            try
            {
                var runner = new RepeaterTaskRunner();
                var asyncTask = runner.RunAsync(task, untilPredicate);
                asyncTask.Wait();
                return asyncTask.Result;
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException;
            }
        }
    }
}
