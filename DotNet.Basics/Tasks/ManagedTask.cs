using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class ManagedTask
    {
        private readonly Action<string> _syncTask;
        private readonly Func<string, Task> _asyncTask;
        private readonly Func<string, TaskEndedReason> _preconditionsMet;

        public delegate void ManagedTaskEventHandler(ManagedTaskEventArgs args);
        public delegate void ManagedTaskEndedEventHandler(ManagedTaskEndedEventArgs args);

        public event ManagedTaskEventHandler TaskStarted;
        public event ManagedTaskEndedEventHandler TaskEnded;

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
            Func<string, TaskEndedReason> preconditionsMet)
        {
            if (syncTask == null) throw new ArgumentNullException(nameof(syncTask));
            if (asyncTask == null) throw new ArgumentNullException(nameof(asyncTask));

            _syncTask = syncTask;
            _asyncTask = asyncTask;
            if (preconditionsMet == null)
                preconditionsMet = rid => TaskEndedReason.AllGood;
            _preconditionsMet = runId =>
            {
                var reason = preconditionsMet.Invoke(runId);
                if (reason != TaskEndedReason.AllGood)
                    FireTaskEnded(runId, reason, null);

                return reason;
            };
            Id = id ?? string.Empty;
        }

        public string Id { get; }

        internal virtual TaskEndedReason PreconditionsMet(string runId)
        {
            return _preconditionsMet(runId);
        }

        internal virtual void Run(string runId)
        {
            try
            {
                FireTaskStarted(runId);
                _syncTask(runId ?? string.Empty);
                FireTaskEnded(runId, TaskEndedReason.AllGood, null);
            }
            catch (Exception e)
            {
                var asAggrE = e;
                while (asAggrE is AggregateException && asAggrE.InnerException != null)
                    asAggrE = asAggrE.InnerException;

                FireTaskEnded(runId, TaskEndedReason.Exception, e);
                throw;
            }
        }

        internal virtual async Task RunAsync(string runId)
        {
            try
            {
                FireTaskStarted(runId);
                await _asyncTask(runId ?? string.Empty).ConfigureAwait(false);
                FireTaskEnded(runId, TaskEndedReason.AllGood, null);
            }
            catch (Exception e)
            {
                var asAggrE = e;
                while (asAggrE is AggregateException && asAggrE.InnerException != null)
                    asAggrE = asAggrE.InnerException;

                FireTaskEnded(runId, TaskEndedReason.Exception, e);
                throw;
            }
        }

        protected void FireTaskStarted(string runId)
        {
            TaskStarted?.Invoke(new ManagedTaskEventArgs(Id, runId));
        }

        protected void FireTaskEnded(string runId, TaskEndedReason reason, Exception e)
        {
            TaskEnded?.Invoke(new ManagedTaskEndedEventArgs(Id, runId, reason, e));
        }
    }
}