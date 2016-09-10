using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public abstract class TaskScheduler
    {
        public delegate void ManagedTaskEventHandler(ManagedTaskEventArgs args);
        public delegate void ManagedTaskEndedEventHandler(ManagedTaskEndedEventArgs args);

        public event ManagedTaskEventHandler TaskStarted;
        public event ManagedTaskEndedEventHandler TaskEnded;

        public abstract bool IsRunning(string taskId);
        protected abstract bool TryAcquireStartTaskLock(string taskId);
        protected abstract bool TryRemoveAcquiredTaskLock(string taskId);

        public bool TryStart(ManagedTask task, string runId, bool scheduleInBackground)
        {
            if (TryAcquireStartTaskLock(task.Id) == false)
            {
                TaskEnded?.Invoke(new ManagedTaskEndedEventArgs(task.Id, runId, TaskEndedReason.AlreadyStarted, null));
                return false;
            }
            if (scheduleInBackground)
            {
                Task.Run(() => Run(task, runId));
                return true;
            }
            return Run(task, runId);

        }

        private bool Run(ManagedTask task, string runId)
        {
            try
            {
                TaskStarted?.Invoke(new ManagedTaskEventArgs(task.Id, runId));
                task.Run(runId);
                TaskEnded?.Invoke(new ManagedTaskEndedEventArgs(task.Id, runId, TaskEndedReason.AllGood, null));
                return true;
            }
            catch (Exception e)
            {
                var asAggrE = e;
                while (asAggrE is AggregateException && asAggrE.InnerException != null)
                    asAggrE = asAggrE.InnerException;

                TaskEnded?.Invoke(new ManagedTaskEndedEventArgs(task.Id, runId, TaskEndedReason.Exception, e));
                throw;
            }
            finally
            {
                TryRemoveAcquiredTaskLock(task.Id);
            }
        }


        public async Task<bool> TryStartAsync(ManagedTask task, string runId, bool scheduleInBackground)
        {
            if (TryAcquireStartTaskLock(task.Id) == false)
            {
                TaskEnded?.Invoke(new ManagedTaskEndedEventArgs(task.Id, runId, TaskEndedReason.AlreadyStarted, null));
                return false;
            }

            if (scheduleInBackground)
            {
                Task.Run(async () => await RunAsync(task, runId).ConfigureAwait(false));
                return true;
            }
            return await RunAsync(task, runId).ConfigureAwait(false);
        }

        public async Task<bool> RunAsync(ManagedTask task, string runId)
        {
            try
            {
                TaskStarted?.Invoke(new ManagedTaskEventArgs(task.Id, runId));
                await task.RunAsync(runId).ConfigureAwait(false);
                TaskEnded?.Invoke(new ManagedTaskEndedEventArgs(task.Id, runId, TaskEndedReason.AllGood, null));
                return true;
            }
            catch (Exception e)
            {
                var asAggrE = e;
                while (asAggrE is AggregateException && asAggrE.InnerException != null)
                    asAggrE = asAggrE.InnerException;

                TaskEnded?.Invoke(new ManagedTaskEndedEventArgs(task.Id, runId, TaskEndedReason.Exception, e));
                throw;
            }
            finally
            {
                TryRemoveAcquiredTaskLock(task.Id);
            }
        }
    }
}
