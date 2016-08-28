using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class AtMostOnceTaskRunner
    {
        private readonly ConcurrentDictionary<string, BackgroundTask> _scheduler;

        public AtMostOnceTaskRunner()
        {
            _scheduler = new ConcurrentDictionary<string, BackgroundTask>();
        }

        public string GetProperty(string taskId, string key)
        {
            BackgroundTask outTask;
            string outProperty;
            if (_scheduler.TryGetValue(taskId, out outTask))
                if (outTask.Properties.TryGetValue(key, out outProperty))
                    return outProperty;
            return null;
        }

        public bool IsRunning(string taskId)
        {
            BackgroundTask outTask;
            return _scheduler.TryGetValue(taskId, out outTask);
        }

        public AtMostOnceTaskRunResult StartTask(string taskId, Func<CancellationToken, Task> task)
        {
            return StartTask(taskId, task, CancellationToken.None);
        }

        public AtMostOnceTaskRunResult StartTask(string taskId, Func<CancellationToken, Task> task, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(taskId))
                throw new ArgumentException($"Id not set in task. Was: {taskId}");

            if (task == null)
                throw new ArgumentNullException(nameof(task));

            //if task is already running
            if (IsRunning(taskId))
                return new AtMostOnceTaskRunResult(taskId, false, "task is already running");

            //lock task for running
            var bgTask = new BackgroundTask(taskId, task, _scheduler);
            var added = _scheduler.TryAdd(taskId, bgTask);
            if (added == false)
                return new AtMostOnceTaskRunResult(taskId, false, "failed to add task");

            //only start task if sucessfully added to scheduler
            bgTask.StartAsync.Invoke(ct);

            return new AtMostOnceTaskRunResult(taskId, IsRunning(taskId), "task ran");
        }
    }
}
