using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class ManagedTask
    {
        private readonly Action<string> _syncTask;
        private readonly Func<string, Task> _asyncTask;

        public delegate void TaskStartedEventHandler(string taskId, string runId);
        public delegate void TaskNotStartedEventHandler(string taskId, string runId, string reason);
        public delegate void TaskFailedEventHandler(string taskId, string runId, Exception lastException);
        public delegate void TaskEndedEventHandler(string taskId, string runId);

        public event TaskStartedEventHandler TaskStarted;
        public event TaskNotStartedEventHandler TaskNotStarted;
        public event TaskFailedEventHandler TaskFailed;
        public event TaskEndedEventHandler TaskEnded;

        public ManagedTask(Action<string> task)
            : this(string.Empty, task)
        {
        }

        public ManagedTask(string id, Action<string> task)
            : this(id, task, runId =>
            {
                task.Invoke(runId);
                return Task.CompletedTask;
            })
        {
        }

        public ManagedTask(Func<string, Task> task)
            : this(string.Empty, task)
        {
        }

        public ManagedTask(string id, Func<string, Task> task)
            : this(id, runId =>
            {
                var asyncTask = task.Invoke(runId);
                asyncTask.Wait();
            }, task)
        {
        }

        public ManagedTask(Action<string> syncTask, Func<string, Task> asyncTask)
            : this(null, syncTask, asyncTask)
        {
        }

        public ManagedTask(string id, Action<string> syncTask, Func<string, Task> asyncTask)
        {
            if (syncTask == null) throw new ArgumentNullException(nameof(syncTask));
            if (asyncTask == null) throw new ArgumentNullException(nameof(asyncTask));
            _syncTask = syncTask;
            _asyncTask = asyncTask;
            Id = id ?? string.Empty;
        }

        public string Id { get; }

        internal virtual bool TryPreconditionsMet(string runId, out string reason)
        {
            reason = "";
            return true;
        }

        internal virtual void Run(string runId = null)
        {
            try
            {
                TaskStarted?.Invoke(Id, runId);
                _syncTask(runId ?? string.Empty);
                TaskEnded?.Invoke(Id, runId);
            }
            catch (Exception e)
            {
                var asAggrE = e;
                while (asAggrE is AggregateException && asAggrE.InnerException != null)
                    asAggrE = asAggrE.InnerException;

                TaskFailed?.Invoke(Id, runId, asAggrE);
                throw;
            }
        }

        internal virtual async Task RunAsync(string runId = null)
        {
            try
            {
                TaskStarted?.Invoke(Id, runId);

                await _asyncTask(runId ?? string.Empty).ConfigureAwait(false);

                TaskEnded?.Invoke(Id, runId);
            }
            catch (Exception e)
            {
                var asAggrE = e;
                while (asAggrE is AggregateException && asAggrE.InnerException != null)
                    asAggrE = asAggrE.InnerException;

                TaskFailed?.Invoke(Id, runId, asAggrE);
                throw;
            }
        }
    }
}
