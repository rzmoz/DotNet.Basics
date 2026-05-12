using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks.Repeating
{
    public static class RepeaterTaskExtensions
    {
        public static Action ToOnceOnly(this Action task)
        {
            return new OnceOnlyTask(task).RunSync;
        }

        public static Func<Task<int>> ToOnceOnly(this Func<Task<int>> task)
        {
            var once = new OnceOnlyTask(task);
            return () => once.RunAsync();
        }

        public static RepeaterTask WithOptions(this RepeaterTask task, Action<RepeatOptions> setOptions)
        {
            if (setOptions == null) throw new ArgumentNullException(nameof(setOptions));
            setOptions(task.Options);
            return task;
        }

        public static Task<bool> UntilNoExceptionsAsync(this RepeaterTask task, CancellationToken cancellationToken = default)
        {
            return task.UntilAsync(e => Task.FromResult(e == null), cancellationToken);
        }

        public static Task<bool> UntilAsync(this RepeaterTask task, Func<Task<bool>> untilPredicate, CancellationToken cancellationToken = default)
        {
            return task.UntilAsync(_ => untilPredicate(), cancellationToken);
        }

        private static Task<bool> UntilAsync(this RepeaterTask task, Func<Exception?, Task<bool>> untilPredicate, CancellationToken cancellationToken = default)
        {
            try
            {
                var runner = new RepeaterTaskRunner();
                return runner.RunAsync(task, untilPredicate, task.Options, cancellationToken);
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
            return task.Until(_ => untilPredicate());
        }

        private static bool Until(this RepeaterTask task, Func<Exception?, bool> untilPredicate)
        {
            try
            {
                var runner = new RepeaterTaskRunner();
                var asyncTask = runner.RunAsync(task, e => Task.FromResult(untilPredicate(e)), task.Options);
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
