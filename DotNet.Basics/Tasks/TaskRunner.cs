using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class TaskRunner
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
            
            if (runAsSingleton)
                task = AsSingleton(task, ct);

            //background task MUST be wrapped outside of singletons as they finish immediately
            if (runInBackground)
                task = InBackground(task, runId, ct);

            try
            {
                TaskStarted?.Invoke(task.Id, runId, true);
                await task.RunAsync(ct).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                TaskEnded?.Invoke(task.Id, runId, e);
                throw;
            }
            TaskEnded?.Invoke(task.Id, runId, null);
        }

        protected ITask InBackground(ITask task, string runId, CancellationToken ct)
        {
            return new SyncTask(() =>
            {
                //dont wait and dont cancel outer task in itself to ensure the actual task gets a chance to cancel on its own
                Task.Run(async () =>
                {
                    try
                    {
                        await task.RunAsync(ct).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        TaskEnded?.Invoke(task.Id, runId, e);
                        //don't throw exception in a bg thread since noone is listening..
                    }

                }, CancellationToken.None);
            }, task.Id);
        }

        protected ITask AsSingleton(ITask task, CancellationToken ct)
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
                    await task.RunAsync(ct).ConfigureAwait(false);
                }
                finally
                {
                    //make sure to unregister task when it's not running anymore
                    Func<CancellationToken, Task> outTask;
                    _singletonScheduler.TryRemove(task.Id, out outTask);
                }
            }, task.Id);
        }
    }
}
