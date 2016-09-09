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
            if(string.IsNullOrWhiteSpace(task.Id))throw new ArgumentException(nameof(task.Id));
        }

        public SingletonTask(string id, Action<string> syncTask, Func<string, TaskEndedReason> preconditionsMet = null) : base(id, syncTask, preconditionsMet)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException(nameof(id));
        }

        public SingletonTask(string id, Func<string, Task> asyncTask, Func<string, TaskEndedReason> preconditionsMet = null) : base(id, asyncTask, preconditionsMet)
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
            return added ? base.PreconditionsMet(runId) : TaskEndedReason.AlreadyStarted;
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
