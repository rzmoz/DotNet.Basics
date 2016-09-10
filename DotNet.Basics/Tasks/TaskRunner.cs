using System;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class TaskRunner
    {
        private static readonly TransientTaskScheduler _transientScheduler = new TransientTaskScheduler();
        private static readonly SingletonTaskScheduler _singletonScheduler = new SingletonTaskScheduler();
        private static ManagedTaskFactory _taskFactory;

        public event TaskScheduler.ManagedTaskEventHandler TaskStarted;
        public event TaskScheduler.ManagedTaskEndedEventHandler TaskEnded;

        public TaskRunner()
        {
            _transientScheduler.TaskStarted += args => { TaskStarted?.Invoke(args); };
            _transientScheduler.TaskEnded += args => { TaskEnded?.Invoke(args); };
            _singletonScheduler.TaskStarted += args => { TaskStarted?.Invoke(args); };
            _singletonScheduler.TaskEnded += args => { TaskEnded?.Invoke(args); };

            _taskFactory = new ManagedTaskFactory();
        }

        public bool IsRunning(string taskId)
        {
            return _transientScheduler.IsRunning(taskId) || _singletonScheduler.IsRunning(taskId);
        }

        public bool TryStart(ManagedTask task, RunMode runMode = RunMode.Transient)
        {
            var runId = NewRunId;

            if (runMode.HasFlag(RunMode.Singleton))
                return _singletonScheduler.TryStart(task, runId, runMode.HasFlag(RunMode.Background));
            return _transientScheduler.TryStart(task, runId, runMode.HasFlag(RunMode.Background));
        }

        public async Task<bool> TryStartAsync(ManagedTask task, RunMode runMode = RunMode.Transient)
        {
            var runId = NewRunId;
            if (runMode.HasFlag(RunMode.Singleton))
                return await _singletonScheduler.TryStartAsync(task, runId, runMode.HasFlag(RunMode.Background)).ConfigureAwait(false);
            return await _transientScheduler.TryStartAsync(task, runId, runMode.HasFlag(RunMode.Background)).ConfigureAwait(false);
        }
        private string NewRunId => $"[{Guid.NewGuid():N}]";
    }
}
