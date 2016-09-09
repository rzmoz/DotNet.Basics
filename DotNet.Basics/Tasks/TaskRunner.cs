using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class TaskRunner
    {
        public event ManagedTask.ManagedTaskEventHandler TaskStarted;
        public event ManagedTask.ManagedTaskEndedEventHandler TaskEnded;

        public void Run(ManagedTask task)
        {
            string runId;
            if (TryInitTask(task, out runId))
                task.Run(runId);
        }

        public async Task RunAsync(ManagedTask task)
        {
            string runId;
            if (TryInitTask(task, out runId))
                await task.RunAsync(runId).ConfigureAwait(false);
        }

        private bool TryInitTask(ManagedTask task, out string runId)
        {
            task.TaskStarted += TaskStarted;
            task.TaskEnded += TaskEnded;
            runId = $"[{Guid.NewGuid():N}]";
            var preconditionReason = task.PreconditionsMet(runId);
            return preconditionReason == TaskEndedReason.AllGood;
        }
    }
}
