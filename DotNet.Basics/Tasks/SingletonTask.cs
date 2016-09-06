using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class SingletonTask : ManagedTask
    {
        private static readonly ConcurrentDictionary<string, string> _singletonScheduler = new ConcurrentDictionary<string, string>();

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

        public SingletonTask(string id, Action<string> syncTask, Func<string, Task> asyncTask) : base(id, syncTask, asyncTask)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException(nameof(id));
        }

        public bool IsRunning()
        {
            return _singletonScheduler.ContainsKey(Id);
        }

        internal override bool TryPreconditionsMet(string runId, out string reason)
        {
            var added = _singletonScheduler.TryAdd(Id, runId ?? string.Empty);
            reason = added
                ? $"Task sucessfully added to scheduler::{Id}"
                : $"Task is already running:{Id}";
            return added;
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
