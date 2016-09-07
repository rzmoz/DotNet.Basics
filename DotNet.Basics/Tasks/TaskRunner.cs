using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class TaskRunner
    {
        public event ManagedTask.TaskStartedEventHandler TaskStarted;
        public event ManagedTask.TaskNotStartedEventHandler TaskNotStarted;
        public event ManagedTask.TaskFailedEventHandler TaskFailed;
        public event ManagedTask.TaskEndedEventHandler TaskEnded;

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
            task.TaskNotStarted += TaskNotStarted;
            task.TaskFailed += TaskFailed;
            task.TaskEnded += TaskEnded;
            runId = Guid.NewGuid().ToString("N");
            var preconditionReason = task.PreconditionsMet(runId);
            return preconditionReason == null;
        }
    }
}
