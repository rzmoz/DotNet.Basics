using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public static class Repeat
    {
        public static TaskVessel<RepeatOptions> Task(Func<CancellationToken, Task> task, RepeatOptions options = null)
        {
            return new TaskVessel<RepeatOptions>(new AsyncTask(task), options);
        }

        public static TaskVessel<RepeatOptions> TaskOnce(Func<CancellationToken, Task> task, RepeatOptions options = null)
        {
            var onceOnlyTask = new OnceOnlyAsyncTask(task);
            return new TaskVessel<RepeatOptions>(new AsyncTask(onceOnlyTask.RunAsync), options);
        }

        public static TaskVessel<RepeatOptions> Task(Action task, RepeatOptions options = null)
        {
            return new TaskVessel<RepeatOptions>(new SyncTask(task), options);
        }

        public static TaskVessel<RepeatOptions> TaskOnce(Action task, RepeatOptions options = null)
        {
            var onceOnlyTask = new OnceOnlySyncTask(task);
            return new TaskVessel<RepeatOptions>(new SyncTask(onceOnlyTask.Run), options);
        }
    }
}
