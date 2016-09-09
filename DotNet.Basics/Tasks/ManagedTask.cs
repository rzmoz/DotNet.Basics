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

        public ManagedTask(string id, Action<string> syncTask, Func<string, TaskEndedReason> preconditionsMet = null)
            : this(id, syncTask, rid => { syncTask.Invoke(rid); return Task.CompletedTask; }, preconditionsMet)
        {
        }

        public ManagedTask(string id, Func<string, Task> asyncTask, Func<string, TaskEndedReason> preconditionsMet = null)
            : this(id, rid => { asyncTask.Invoke(rid).Wait(); }, asyncTask, preconditionsMet)
        {
        }

        private ManagedTask(string id, Action<string> syncTask, Func<string, Task> asyncTask, Func<string, TaskEndedReason> preconditionsMet)
        {
            Id = id ?? string.Empty;
            _syncTask = syncTask ?? VoidSyncTask;
            _asyncTask = asyncTask ?? VoidAsyncTask;
            if (preconditionsMet == null)
                _preconditionsMet = rid => TaskEndedReason.AllGood;
            else
                _preconditionsMet = runId =>
                {
                    var reason = preconditionsMet.Invoke(runId);
                    if (reason != TaskEndedReason.AllGood)
                        FireTaskEnded(runId, reason, null);

                    return reason;
                };
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

        private void VoidSyncTask(string runId)
        { }
        private Task VoidAsyncTask(string runId)
        {
            return Task.CompletedTask;
        }
    }
}