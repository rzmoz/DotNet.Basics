using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public static class Repeat
    {
        private static readonly TaskFactory _taskFactory = new TaskFactory();

        public static RunTask<RepeatOptions> Task(Func<CancellationToken, Task> task, RepeatOptions options = null)
        {
            return _taskFactory.Create(task, options);
        }

        public static RunTask<RepeatOptions> TaskOnce(Func<CancellationToken, Task> task, RepeatOptions options = null)
        {
            return _taskFactory.Create(async ct => await new OnceOnlyAsyncTask(task).RunAsync(ct).ConfigureAwait(false), options);
        }

        public static RunTask<RepeatOptions> Task(Action task, RepeatOptions options = null)
        {
            return _taskFactory.Create(task, options);
        }

        public static RunTask<RepeatOptions> TaskOnce(Action task, RepeatOptions options = null)
        {
            return _taskFactory.Create(() => new OnceOnlySyncTask(task).Run(), options);
        }
    }
}
