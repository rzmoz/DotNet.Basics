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

        public bool IsRunning(string taskId)
        {
            return _runningTasks.Get(taskId) != null;
        }

        public async Task<AtMostOnceTaskRunResult> RunAsync(string taskId, Func<CancellationToken, Task> task)
        {
            return await RunAsync(taskId, task, string.Empty, CancellationToken.None).ConfigureAwait(false);
        }
        public async Task<AtMostOnceTaskRunResult> RunAsync(string taskId, Func<CancellationToken, Task> task, TimeSpan taskTimeout)
        {
            return await RunAsync(taskId, task, string.Empty, CancellationToken.None, taskTimeout).ConfigureAwait(false);
        }
        public async Task<AtMostOnceTaskRunResult> RunAsync(string taskId, Func<CancellationToken, Task> task, CancellationToken ct)
        {
            return await RunAsync(taskId, task, string.Empty, ct).ConfigureAwait(false);
        }
        public async Task<AtMostOnceTaskRunResult> RunAsync(string taskId, Func<CancellationToken, Task> task, CancellationToken ct, TimeSpan taskTimeout)
        {
            return await RunAsync(taskId, task, string.Empty, ct, taskTimeout).ConfigureAwait(false);
        }
        public async Task<AtMostOnceTaskRunResult> RunAsync(string taskId, Func<CancellationToken, Task> task, string cacheValue, CancellationToken ct)
        {
            return await RunAsync(taskId, task, cacheValue, ct, 5.Seconds()).ConfigureAwait(false);
        }
        public async Task<AtMostOnceTaskRunResult> RunAsync(string taskId, Func<CancellationToken, Task> task, string cacheValue, CancellationToken ct, TimeSpan taskTimeout)
        {
            if (string.IsNullOrWhiteSpace(taskId))
                throw new ArgumentException($"Id not set in task. Was: {taskId}");

            if (task == null)
                throw new ArgumentNullException(nameof(task));

            //if task is already running
            if (_runningTasks.Get(taskId) != null)
                return new AtMostOnceTaskRunResult(taskId, false, "task is already running");

            //lock task for running
            var item = new CacheItem<string>(taskId, cacheValue ?? string.Empty, ExpirationMode.Absolute, taskTimeout);
            var added = _runningTasks.Add(item);

            if (added == false)
                return new AtMostOnceTaskRunResult(taskId, false, "failed to add task to scheduler");

            try
            {
                var runningTask = task.Invoke(ct);
                var refreshLockTokenSource = new CancellationTokenSource();
                var refreshLockTask = Task.Factory.StartNew(async () =>
                {
                    while (true)
                    {
                        var refreshInterval = (int)(taskTimeout.Ticks / (2.0 / 3.0));
                        // ReSharper disable once MethodSupportsCancellation
                        await Task.Delay(TimeSpan.FromTicks(refreshInterval)).ConfigureAwait(false);
                        _runningTasks.Update(taskId, id => id);
                    }
                }, ct);

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
