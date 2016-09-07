using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class ManagedTask
    {
        private readonly Action<string> _syncTask;
        private readonly Func<string, Task> _asyncTask;
        private readonly Func<string, string> _preconditionsMet;

        public delegate void TaskStartedEventHandler(string taskId, string runId);

        public delegate void TaskNotStartedEventHandler(string taskId, string runId, string reason);

        public delegate void TaskFailedEventHandler(string taskId, string runId, Exception e);

        public delegate void TaskEndedEventHandler(string taskId, string runId);

        public event TaskStartedEventHandler TaskStarted;
        public event TaskNotStartedEventHandler TaskNotStarted;
        public event TaskFailedEventHandler TaskFailed;
        public event TaskEndedEventHandler TaskEnded;

        public ManagedTask(ManagedTask task)
            : this(task.Id, task.Run, task.RunAsync, task.PreconditionsMet)
        {
        }

        public ManagedTask(Action<string> task)
            : this(string.Empty, task)
        {
        }

        public ManagedTask(string id, Action<string> task)
            : this(id, task, runId =>
            {
                task.Invoke(runId);
                return Task.CompletedTask;
            }, null)
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
            }, task, null)
        {
        }

        private ManagedTask(string id, Action<string> syncTask, Func<string, Task> asyncTask,
            Func<string, string> preconditionsMet)
        {
            if (syncTask == null) throw new ArgumentNullException(nameof(syncTask));
            if (asyncTask == null) throw new ArgumentNullException(nameof(asyncTask));
            _syncTask = syncTask;
            _asyncTask = asyncTask;
            _preconditionsMet = runId =>
            {
                var reason = preconditionsMet?.Invoke(runId);

                if (reason != null)
                    FireTaskNotStarted(Id, runId, reason);

                return reason;
            };
            Id = id ?? string.Empty;
        }

        public string Id { get; }

        internal virtual string PreconditionsMet(string runId)
        {
            return _preconditionsMet(runId);
        }

        internal virtual void Run(string runId)
        {
            try
            {
                FireTaskStarted(Id, runId);
                _syncTask(runId ?? string.Empty);
                FireTaskEnded(Id, runId);
            }
            catch (Exception e)
            {
                var asAggrE = e;
                while (asAggrE is AggregateException && asAggrE.InnerException != null)
                    asAggrE = asAggrE.InnerException;

                FireTaskFailed(Id, runId, asAggrE);
                throw;
            }
        }

        internal virtual async Task RunAsync(string runId)
        {
            try
            {
                FireTaskStarted(Id, runId);
                await _asyncTask(runId ?? string.Empty).ConfigureAwait(false);
                FireTaskEnded(Id, runId);
            }
            catch (Exception e)
            {
                var asAggrE = e;
                while (asAggrE is AggregateException && asAggrE.InnerException != null)
                    asAggrE = asAggrE.InnerException;

                FireTaskFailed(Id, runId, asAggrE);
                throw;
            }
        }

        protected void FireTaskStarted(string taskId, string runId)
        {
            TaskStarted?.Invoke(Id, runId);
        }

        protected void FireTaskNotStarted(string taskId, string runId, string reason)
        {
            TaskNotStarted?.Invoke(Id, runId, reason);
        }

        protected void FireTaskFailed(string taskId, string runId, Exception e)
        {
            TaskFailed?.Invoke(Id, runId, e);
        }

        protected void FireTaskEnded(string taskId, string runId)
        {
            TaskEnded?.Invoke(Id, runId);
        }
    }
}