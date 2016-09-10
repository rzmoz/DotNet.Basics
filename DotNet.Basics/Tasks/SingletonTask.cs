using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class SingletonTask : ManagedTask
    {
        private static readonly ConcurrentDictionary<string, string> _singletonScheduler = new ConcurrentDictionary<string, string>();

        public SingletonTask(ManagedTask task) : base(task)
        {
            if (task.Id == null) throw new ArgumentNullException(nameof(task.Id));
            if (string.IsNullOrWhiteSpace(task.Id)) throw new ArgumentException(nameof(task.Id));
        }

        public SingletonTask(string id, Action<string> syncTask) : base(id, syncTask)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException(nameof(id));
        }

        public SingletonTask(string id, Func<string, Task> asyncTask) : base(id, asyncTask)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException(nameof(id));
        }

        public override bool IsRunning()
        {
            return _singletonScheduler.ContainsKey(Id);
        }

        internal override void Run(string runId)
        {
            try
            {
                base.Run(runId ?? string.Empty);
            }
            finally
            {
                //make sure to unregister task when it's not running anymore
                _singletonScheduler.TryRemove(Id, out runId);
            }
        }

        internal override async Task RunAsync(string runId)
        {
            try
            {
                await base.RunAsync(runId ?? string.Empty).ConfigureAwait(false);
            }
            finally
            {
                //make sure to unregister task when it's not running anymore
                _singletonScheduler.TryRemove(Id, out runId);
            }
        }

        internal override bool TryAcquireStartlock(string runId)
        {
            var canStart = _singletonScheduler.TryAdd(Id, runId ?? string.Empty);
            Console.WriteLine($"TryAcquireStartTaskLock:{Id} - {canStart}");
            return canStart;
        }
    }
}
