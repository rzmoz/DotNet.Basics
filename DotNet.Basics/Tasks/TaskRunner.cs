using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class TaskRunner
    {
        private static readonly IDictionary<RunScope, TaskScheduler> _schedulers;
        private static ManagedTaskFactory _taskFactory;

        public event TaskScheduler.ManagedTaskEventHandler TaskStarted;
        public event TaskScheduler.ManagedTaskEndedEventHandler TaskEnded;

        static TaskRunner()
        {
            _schedulers = new Dictionary<RunScope, TaskScheduler>
            {
                {RunScope.Singleton, new SingletonTaskScheduler()},
                {RunScope.Transient, new TransientTaskScheduler()}
            };
        }

        public TaskRunner()
        {
            foreach (var taskScheduler in _schedulers.Values)
            {
                taskScheduler.TaskStarted += args => { TaskStarted?.Invoke(args); };
                taskScheduler.TaskEnded += args => { TaskEnded?.Invoke(args); };
            }
            _taskFactory = new ManagedTaskFactory();
        }

        public bool IsRunning(string taskId)
        {
            return _schedulers.Values.Any(ts => ts.IsRunning(taskId));
        }

        public bool TryStart(Action<string> syncTask, RunThread runThread = RunThread.Current, RunScope runScope = RunScope.Transient)
        {
            var task = _taskFactory.Create<ManagedTask>(syncTask);
            return TryStart(task, runThread, runScope);
        }
        public async Task<bool> TryStartAsync(Func<string, Task> asyncTask, RunThread runThread = RunThread.Current, RunScope runScope = RunScope.Transient)
        {
            var task = _taskFactory.Create<ManagedTask>(asyncTask);
            return await TryStartAsync(task, runThread, runScope).ConfigureAwait(false);
        }
        public bool TryStart(string taskId, Action<string> syncTask, RunThread runThread = RunThread.Current, RunScope runScope = RunScope.Transient)
        {
            var task = _taskFactory.Create<ManagedTask>(taskId, syncTask);
            return TryStart(task, runThread, runScope);
        }
        public async Task<bool> TryStartAsync(string taskId, Func<string, Task> asyncTask, RunThread runThread = RunThread.Current, RunScope runScope = RunScope.Transient)
        {
            var task = _taskFactory.Create<ManagedTask>(taskId, asyncTask);
            return await TryStartAsync(task, runThread, runScope).ConfigureAwait(false);
        }

        public bool TryStart(ManagedTask task, RunThread runThread = RunThread.Current, RunScope runScope = RunScope.Transient)
        {
            return _schedulers[runScope].TryStart(task, runThread);
        }

        public async Task<bool> TryStartAsync(ManagedTask task, RunThread runThread = RunThread.Current, RunScope runScope = RunScope.Transient)
        {
            return await _schedulers[runScope].TryStartAsync(task, runThread).ConfigureAwait(false);
        }
    }
}
