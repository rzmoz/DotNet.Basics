using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class AtMostOnceTaskRunner
    {
        private readonly ConcurrentDictionary<string, BackgroundTask> _scheduler;

        public delegate void TaskFailedEventHandler(string taskId, Exception e);
        public TaskFailedEventHandler TaskFailed;

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

        public AtMostOnceTaskRunResult StartTask(string taskId, Func<CancellationToken, Task> task, Action<string, Exception> taskFailedCallback = null)
        {
            return StartTask(taskId, task, CancellationToken.None, taskFailedCallback);
        }

        public AtMostOnceTaskRunResult StartTask(string taskId, Func<CancellationToken, Task> task, CancellationToken ct, Action<string, Exception> taskFailedCallback = null)
        {
            if (string.IsNullOrWhiteSpace(taskId))
                throw new ArgumentException($"Id not set in task. Was: {taskId}");

            if (task == null)
                throw new ArgumentNullException(nameof(task));

            //if task is already running
            if (IsRunning(taskId))
                return new AtMostOnceTaskRunResult(taskId, false, "task is already running");

            //lock task for running
            var bgTask = new BackgroundTask(taskId, task);
            var added = _scheduler.TryAdd(taskId, bgTask);
            if (added == false)
                return new AtMostOnceTaskRunResult(taskId, false, "failed to add task to scheduler - please try again");

            //only start task if sucessfully added to scheduler
            //we dont await completion and don't pass a cancellation token since it's important that exception handling is always performed
            Task.Run(async () =>
            {
                try
                {
                    await bgTask.StartAsync.Invoke(ct).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    TaskFailed?.Invoke(bgTask.Id, e);
                    taskFailedCallback?.Invoke(bgTask.Id, e);
                }
                finally
                {
                    //make sure to unregister task when it's not running anymore
                    ((IDictionary<string, BackgroundTask>)_scheduler).Remove(bgTask.Id);
                }
            });

            return new AtMostOnceTaskRunResult(taskId, true, "task started");
        }
    }
}
