using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class SingletonTask : ManagedTask
    {
        private static readonly ConcurrentDictionary<string, string> _singletonScheduler = new ConcurrentDictionary<string, string>();

        public SingletonTask(ManagedTask task) : base(task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (task.Id == null) throw new ArgumentNullException(nameof(task.Id));
            if (string.IsNullOrWhiteSpace(task.Id)) throw new ArgumentException(nameof(task.Id));
        }

        public SingletonTask(string id, Action<string> task) : base(id, task)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException(nameof(id));
        }

        public SingletonTask(string id, Func<string, Task> task) : base(id, task)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException(nameof(id));
        }

        public bool IsRunning()
        {
            return _singletonScheduler.ContainsKey(Id);
        }

        internal override TaskEndedReason PreconditionsMet(string runId)
        {
            var added = _singletonScheduler.TryAdd(Id, runId ?? string.Empty);
            var endReason = added ? TaskEndedReason.AllGood : TaskEndedReason.AlreadyStarted;
            Debug.WriteLine($"SingletonTask {Id}:{runId} preconditions result: {endReason}");
            return endReason;
        }

        internal override void Run(string runId = null)
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

        internal override async Task RunAsync(string runId = null)
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
    }
}
