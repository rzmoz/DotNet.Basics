using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class SyncTask : SyncTask<TaskOptions>
    {
        public SyncTask(Action task, TaskOptions options = null, string id = null) : base(task, options, id)
        {
        }
    }
    public class SyncTask<T> : RunTask<T> where T : TaskOptions, new()
    {
        private readonly Action _syncTask;
        private readonly Func<Task> _asyncTask;

        public SyncTask(Action task, T options = default(T), string id = null) : base(options, id)
        {
            _syncTask = task;
            _asyncTask = async () => task.Invoke();
        }

        public override void Run()
        {
            _syncTask.Invoke();
        }

        public override async Task RunAsync(CancellationToken ct = new CancellationToken())
        {
            await _asyncTask.Invoke().ConfigureAwait(false);
        }
    }
}
