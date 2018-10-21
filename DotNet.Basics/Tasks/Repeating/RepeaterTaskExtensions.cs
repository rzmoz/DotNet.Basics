using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks.Repeating
{
    public static class RepeaterTaskExtensions
    {
        public static Action ToOnceOnly(this Action task)
        {
            return new OnceOnlyTask(task).RunSync;
        }

        public static Func<Task> ToOnceOnly(this Func<Task> task)
        {
            return new OnceOnlyTask(task).RunAsync;
        }

        public static RepeaterTask WithOptions(this RepeaterTask task, Action<RepeatOptions> setOptions)
        {
            if (setOptions == null) throw new ArgumentNullException(nameof(setOptions));
            setOptions(task.Options);
            return task;
        }

        public static Task<bool> UntilNoExceptionsAsync(this RepeaterTask task)
        {
            return task.UntilAsync(e => e == null);
        }

        public static Task<bool> UntilAsync(this RepeaterTask task, Func<bool> untilPredicate)
        {
            return task.UntilAsync(e => untilPredicate());
        }

        private static Task<bool> UntilAsync(this RepeaterTask task, Func<Exception, bool> untilPredicate)
        {
            try
            {
                var runner = new RepeaterTaskRunner();
                return runner.RunAsync(task, untilPredicate, task.Options);
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException ?? ae;
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
                var asyncTask = runner.RunAsync(task, untilPredicate, task.Options);
                asyncTask.Wait();
                return asyncTask.Result;
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException ?? ae;
            }
        }
    }
}
