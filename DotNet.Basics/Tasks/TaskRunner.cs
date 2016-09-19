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

        public bool TryStart(Action<string> syncTask, RunMode runMode = RunMode.Transient)
        {
            var task = _taskFactory.Create<ManagedTask>(syncTask);
            return TryStart(task, runMode);
        }
        public async Task<bool> TryStartAsync(Func<string, Task> asyncTask, RunMode runMode = RunMode.Transient)
        {
            var task = _taskFactory.Create<ManagedTask>(asyncTask);
            return await TryStartAsync(task, runMode).ConfigureAwait(false);
        }
        public bool TryStart(string taskId, Action<string> syncTask, RunMode runMode = RunMode.Transient)
        {
            var task = _taskFactory.Create<ManagedTask>(taskId, syncTask);
            return TryStart(task, runMode);
        }
        public async Task<bool> TryStartAsync(string taskId, Func<string, Task> asyncTask, RunMode runMode = RunMode.Transient)
        {
            var task = _taskFactory.Create<ManagedTask>(taskId, asyncTask);
            return await TryStartAsync(task, runMode).ConfigureAwait(false);
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
