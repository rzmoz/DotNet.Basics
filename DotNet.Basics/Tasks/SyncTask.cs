using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class SyncTask : ITask
    {
        private readonly Action _syncTask;
        private readonly Func<Task> _asyncTask;

        public SyncTask(Action task)
        {
            _syncTask = task;
            _asyncTask = () => { task.Invoke(); return Task.CompletedTask; };
        }

        public void Run()
        {
            _syncTask.Invoke();
        }

        public async Task RunAsync(CancellationToken ct = new CancellationToken())
        {
            await _asyncTask.Invoke().ConfigureAwait(false);
        }
    }
}
