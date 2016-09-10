using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class ManagedTask
    {
        private readonly Action<string> _syncTask;
        private readonly Func<string, Task> _asyncTask;

        public ManagedTask(ManagedTask task)
            : this(task.Id, task.Run, task.RunAsync)
        {
        }

        public ManagedTask(string id, Action<string> syncTask)
            : this(id, syncTask, rid =>
            {
                syncTask.Invoke(rid);
                return Task.CompletedTask;
            })
        {
        }

        public ManagedTask(string id, Func<string, Task> asyncTask)
            : this(id, rid => { asyncTask.Invoke(rid).Wait(); }, asyncTask)
        {
        }

        private ManagedTask(string id, Action<string> syncTask, Func<string, Task> asyncTask)
        {
            Id = id ?? string.Empty;
            _syncTask = syncTask ?? VoidSyncTask;
            _asyncTask = asyncTask ?? VoidAsyncTask;
        }

        public string Id { get; }
        
        internal virtual void Run(string runId)
        {
            _syncTask(runId ?? string.Empty);
        }

        internal virtual async Task RunAsync(string runId)
        {
            await _asyncTask(runId ?? string.Empty).ConfigureAwait(false);
        }

        private void VoidSyncTask(string runId)
        { }
        private Task VoidAsyncTask(string runId)
        {
            return Task.CompletedTask;
        }
    }
}