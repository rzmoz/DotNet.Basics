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

        protected abstract bool InnerTryAcquireStartTaskLock(string taskId);
        protected abstract bool TryRemoveTaskLock(string taskId);

        private string NewRunId => $"[{Guid.NewGuid():N}]";

        public bool TryStart(ManagedTask task, RunThread runThread)
        {
            var runId = NewRunId;

            if (TryAcquireTaskLock(task.Id, runId) == false)
                return false;
            if (runThread == RunThread.Background)
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
                ExceptionCaughtHandling(task.Id, runId, e);
                throw;
            }
            finally
            {
                TryRemoveTaskLock(task.Id);
            }
        }


        public async Task<bool> TryStartAsync(ManagedTask task, RunThread runThread)
        {
            var runId = NewRunId;

            if (TryAcquireTaskLock(task.Id, runId) == false)
                return false;
            if (runThread == RunThread.Background)
            {
                Task.Run(async () => await RunAsync(task, runId).ConfigureAwait(false));
                return true;
            }
            return await RunAsync(task, runId).ConfigureAwait(false);
        }

        private async Task<bool> RunAsync(ManagedTask task, string runId)
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
                ExceptionCaughtHandling(task.Id, runId, e);
                throw;
            }
            finally
            {
                TryRemoveTaskLock(task.Id);
            }
        }

        private void ExceptionCaughtHandling(string taskId, string runId, Exception e)
        {
            var asAggrE = e;
            while (asAggrE is AggregateException && asAggrE.InnerException != null)
                asAggrE = asAggrE.InnerException;

            TaskEnded?.Invoke(new ManagedTaskEndedEventArgs(taskId, runId, TaskEndedReason.Exception, e));
        }

        private bool TryAcquireTaskLock(string taskId, string runId)
        {
            if (InnerTryAcquireStartTaskLock(taskId) == false)
            {
                TaskEnded?.Invoke(new ManagedTaskEndedEventArgs(taskId, runId, TaskEndedReason.AlreadyStarted, null));
                return false;
            }
            return true;
        }

    }
}
