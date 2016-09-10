using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class TaskRunner
    {
        public event ManagedTask.ManagedTaskEventHandler TaskStarted;
        public event ManagedTask.ManagedTaskEndedEventHandler TaskEnded;

        protected ManagedTaskFactory TaskFactory { get; }

        public TaskRunner()
        {
            TaskFactory = new ManagedTaskFactory();
        }

        public void Run(ManagedTask task)
        {
            var runId = InitTask(task);
            if (task.TryAcquireStartlock(runId) == false)
            {
                TaskEnded?.Invoke(new ManagedTaskEndedEventArgs(task.Id, runId, TaskEndedReason.AlreadyStarted, null));
                return;
            }
            task.Run(runId);
        }

        public async Task RunAsync(ManagedTask task)
        {
            var runId = InitTask(task);
            if (task.TryAcquireStartlock(runId)== false)
            {
                TaskEnded?.Invoke(new ManagedTaskEndedEventArgs(task.Id,runId,TaskEndedReason.AlreadyStarted, null));
                return;
            }
            await task.RunAsync(runId).ConfigureAwait(false);
        }

        private string InitTask(ManagedTask task)
        {
            task.TaskStarted += TaskStarted;
            task.TaskEnded += TaskEnded;
            return $"[{Guid.NewGuid():N}]";
        }
    }
}
