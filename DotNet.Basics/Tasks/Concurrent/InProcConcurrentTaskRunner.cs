using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tasks.Concurrent
{
    public class InProcConcurrentTaskRunner : IConcurrentTaskRunner
    {
        private readonly TaskSyncProvider _taskSyncProvider;
        private static readonly IDictionary<string, object> _syncRoots = new Dictionary<string, object>();

        private const string _taskDoneKey = "__taskDone";
        private const string _taskDoneValue = "__true";


        public InProcConcurrentTaskRunner()
        {
            _taskSyncProvider = new TaskSyncProvider();
        }
        public InProcConcurrentTaskRunner(TimeSpan lockTimeout)
        {
            _taskSyncProvider = new TaskSyncProvider(lockTimeout);
        }

        public bool IsLocked(string taskName)
        {
            return _taskSyncProvider.IsLocked(taskName);
        }
        
        public StartResult RunAtMostOnceInBackground(string taskName, Action<KeyValueCollection> action)
        {
            var task = new AsyncTask(taskName, action);
            return RunAtMostOnceInBackground(task);
        }

        public StartResult RunAtMostOnceInBackground(string taskName, Func<KeyValueCollection, Task> action)
        {
            var task = new AsyncTask(taskName, action);
            return RunAtMostOnceInBackground(task);
        }

        public StartResult RunAtMostOnceInBackground(IAsyncTask task)
        {
            var syncRoot = GetSyncRoot(task);

            if (Monitor.TryEnter(syncRoot, 5.Seconds()))
            {
                try
                {
                    var isLockedAlready = IsLocked(task.Name);
                    if (isLockedAlready)
                        return StartResult.AlreadyRunning;

                    task.Metadata[_taskDoneValue] = string.Empty;//reset done status
                    //acquire lock - will be release if task dies
                    RenewLock(task);

                    //we deliberately don't wait for this task
                    Task.Run(async () =>
                    {
                        var renewInterval = TimeSpan.FromTicks(_taskSyncProvider.LockTimeout.Ticks / 3 * 2);
                        //set to two thirds of lock timeout
                        using (new Timer(RenewLock, task, TimeSpan.Zero, renewInterval))
                        {
                            await task.RunAsync().ConfigureAwait(false);
                        }

                        //esnure lock is released on exception but we don't want to catch exceptions since we don't want to mark the task as done - the task should just quit
                        _taskSyncProvider.RemoveLock(task.Name);
                        //we should never enter here if task throws exception
                        //set done state as the final thing to ensure it's never set unless completey finished
                        task.Metadata[_taskDoneKey] = _taskDoneValue;
                        _taskSyncProvider.SetTaskInfo(task);
                    }).ConfigureAwait(false);

                    return StartResult.Started;
                }
                finally
                {
                    Monitor.Exit(syncRoot);
                }
            }

            // failed to get lock
            return StartResult.AlreadyRunning;
        }

        public StartResult EraseIfNotRunning(string taskName)
        {
            return EraseIfNotRunning(new TaskId(taskName));
        }

        public StartResult EraseIfNotRunning(ITaskId taskId)
        {
            var syncRoot = GetSyncRoot(taskId);

            if (Monitor.TryEnter(syncRoot, 5.Seconds()))
            {
                try
                {
                    var isLockedAlready = IsLocked(taskId.Name);
                    if (isLockedAlready)
                        return StartResult.AlreadyRunning;
                    _taskSyncProvider.RemoveLock(taskId.Name);
                    _taskSyncProvider.RemoveTaskInfo(taskId.Name);
                    return StartResult.Started;
                }
                finally
                {
                    Monitor.Exit(syncRoot);
                }
            }
            return StartResult.AlreadyRunning;
        }

        public void ForceReleaseLock(string taskName)
        {
            _taskSyncProvider.RemoveLock(taskName);
        }

        private object GetSyncRoot(ITaskId taskId)
        {
            if (taskId == null) throw new ArgumentNullException(nameof(taskId));

            //ensure syncroot exists for task - no lock creation to avoid race conditions
            try
            {
                _syncRoots.Add(taskId.Id, new object());
            }
            catch (ArgumentException)
            {
                /*ignore*/
            }

            return _syncRoots[taskId.Id];
        }

        public ITaskInfo GetTaskInfo(string taskName)
        {
            return _taskSyncProvider.GetTaskInfo(taskName);
        }

        public TaskStatus GetStatus(string taskName)
        {
            var taskInfo = GetTaskInfo(taskName);

            TaskState taskState;

            if (IsLocked(taskName))
                taskState = TaskState.Processing;
            else if (taskInfo == null)
                taskState = TaskState.NotFound;
            else if (taskInfo.Metadata[_taskDoneKey] == _taskDoneValue)
                taskState = TaskState.Done;
            else
                taskState = TaskState.Error;
            return new TaskStatus(taskState, taskInfo);
        }

        private void RenewLock(object obj)
        {
            var task = obj as ITaskInfo;
            if (task == null)
                throw new ArgumentException("obj is not an ITask");

            _taskSyncProvider.SetLock(task.Name);
            _taskSyncProvider.SetTaskInfo(task);
        }
    }
}
