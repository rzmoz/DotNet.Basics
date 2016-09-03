using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class TaskRunner<T> where T : TaskOptions, new()
    {
        private static readonly ConcurrentDictionary<string, Func<CancellationToken, Task>> _singletonScheduler;

        public delegate void TaskStartedEventHandler(string taskId, string runId, bool started);
        public delegate void TaskEndedEventHandler(string taskId, string runId, Exception lastException);

        public TaskStartedEventHandler TaskStarted;
        public TaskEndedEventHandler TaskEnded;

        static TaskRunner()
        {
            _singletonScheduler = new ConcurrentDictionary<string, Func<CancellationToken, Task>>();
        }

        public bool IsRunning(string taskId)
        {
            return _singletonScheduler.ContainsKey(taskId);
        }

        protected async Task RunAsync(ITask task, CancellationToken ct = default(CancellationToken), bool runAsSingleton = false, bool runInBackground = false)
        {
            var runId = Guid.NewGuid().ToString("N");

            if (runInBackground)
                task = InBackground(task, ct);

            if (runAsSingleton)
                task = AsSingleton(task, ct);

            await task.RunAsync(ct).ConfigureAwait(false);
        }

        protected ITask InBackground(ITask task, CancellationToken ct = default(CancellationToken))
        {
            return new SyncTask(() =>
            {
                //dont wait and dont cancel outer task in itself to ensure the actual task gets a chance to cancel on its own
                Task.Run(async () =>
                {
                    await RunAsync(task, ct).ConfigureAwait(false);
                }, CancellationToken.None);
            }, task.Id);
        }

        protected ITask AsSingleton(ITask task, CancellationToken ct = default(CancellationToken))
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            //if task is already running
            if (IsRunning(task.Id))
                return new SyncTask(() => { }, task.Id);

            //try to lock task for running
            var added = _singletonScheduler.TryAdd(task.Id, task.RunAsync);
            if (added == false)
                return new SyncTask(() => { }, task.Id);

            return new AsyncTask(async ctx =>
            {
                try
                {

                    await RunAsync(task.Id, task.RunAsync, ct).ConfigureAwait(false);
                }
                finally
                {
                    //make sure to unregister task when it's not running anymore
                    ((IDictionary<string, TaskVessel<T>>)_singletonScheduler).Remove(task.Id);
                }
            }, task.Id);
        }

        protected async Task RunAsync(string taskId, Func<CancellationToken, Task> task, CancellationToken ct = default(CancellationToken))
        {
            var runId = Guid.NewGuid().ToString("N");

            try
            {
                TaskStarted?.Invoke(taskId, runId, true);
                await task.Invoke(ct).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                TaskEnded?.Invoke(taskId, runId, e);
                throw;
            }
            TaskEnded?.Invoke(taskId, runId, null);
        }
    }
}
