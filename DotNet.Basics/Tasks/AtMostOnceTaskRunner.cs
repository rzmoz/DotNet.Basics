using System;
using System.Threading;
using System.Threading.Tasks;
using CacheManager.Core;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tasks
{

    public class AtMostOnceTaskRunner
    {
        private readonly ICacheManager<string> _runningTasks;

        //defaults to in mem cache manager for in-proc support. Use distributed cache for out-of-proc support
        public AtMostOnceTaskRunner()
            : this(CacheFactory.Build<string>(p => p.WithSystemRuntimeCacheHandle()))
        {
        }

        public AtMostOnceTaskRunner(ICacheManager<string> cacheManager)
        {
            _runningTasks = cacheManager;
        }

        public string GetTaskValue(string id)
        {
            return _runningTasks.Get(id);
        }

        public async Task<AtMostOnceTaskRunResult> RunAsync(string taskId, Func<Task> task)
        {
            return await RunAsync(taskId, task, string.Empty).ConfigureAwait(false);
        }
        public async Task<AtMostOnceTaskRunResult> RunAsync(string taskId, Func<Task> task, string cacheValue)
        {
            return await RunAsync(taskId, task, cacheValue, 5.Seconds()).ConfigureAwait(false);
        }
        public async Task<AtMostOnceTaskRunResult> RunAsync(string taskId, Func<Task> task, string cacheValue, TimeSpan expirationTimeout)
        {
            if (string.IsNullOrWhiteSpace(taskId))
                throw new ArgumentException($"Id not set in task. Was: {taskId}");

            if (task == null)
                throw new ArgumentNullException(nameof(task));

            //if task is already running
            if (_runningTasks.Get(taskId) != null)
                return new AtMostOnceTaskRunResult(taskId, false, "task is already running");

            //lock task for running
            var item = new CacheItem<string>(taskId, cacheValue ?? string.Empty, ExpirationMode.Absolute, expirationTimeout);
            var added = _runningTasks.Add(item);

            if (added == false)
                return new AtMostOnceTaskRunResult(taskId, false, "failed to add task to scheduler");

            try
            {
                var runningTask = task.Invoke();
                var refreshLockTokenSource = new CancellationTokenSource();
                var refreshLockTask = Task.Factory.StartNew(async () =>
                {
                    while (true)
                    {
                        var refreshInterval = (int)(expirationTimeout.Ticks / (2.0 / 3.0));
                        await Task.Delay(TimeSpan.FromTicks(refreshInterval), refreshLockTokenSource.Token).ConfigureAwait(false);
                        _runningTasks.AddOrUpdate(item, id => id);
                    }
                }, refreshLockTokenSource.Token);

                await Task.WhenAll(runningTask).ConfigureAwait(false);//wait til main task is completed
                refreshLockTokenSource.Cancel();
            }
            finally
            {
                _runningTasks.Remove(item.Key);//release lock when done    
            }
            return new AtMostOnceTaskRunResult(taskId, true, "task ran");
        }
    }
}
