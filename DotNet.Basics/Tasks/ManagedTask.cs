using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class ManagedTask
    {
        private readonly Action _syncTask;
        private readonly Func<CancellationToken, Task> _asyncTask;

        public ManagedTask(Func<CancellationToken, Task> task, string id = null)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            _syncTask = () =>
            {
                var asyncTask = task.Invoke(CancellationToken.None);
                asyncTask.Wait();
            };
            _asyncTask = task;
            Id = id ?? string.Empty;
        }
        public ManagedTask(Action task, string id = null)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            _syncTask = task;
            _asyncTask = ct =>
            {
                task.Invoke();
                return Task.CompletedTask;
            };
            Id = id;
        }

        public string Id { get; }

        public virtual void Run()
        {
            _syncTask();
        }

        public virtual async Task RunAsync(CancellationToken ct = default(CancellationToken))
        {
            await _asyncTask(ct).ConfigureAwait(false);
        }
    }
}
