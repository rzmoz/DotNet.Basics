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

        public async Task<bool> RunAsync(AtMostOnceTask task)
        {
            return await RunAsync(task, 5.Seconds()).ConfigureAwait(false);
        }
        public async Task<bool> RunAsync(AtMostOnceTask task, TimeSpan expirationTimeout)
        {
            if (task == null)
                return false;

            if (string.IsNullOrWhiteSpace(task.Id))
                throw new ArgumentException($"Id not set in task. Was: {task.Id}");

            //if task is already running
            if (_runningTasks.Get(task.Id) != null)
                return false;

            //lock task for running
            var item = new CacheItem<string>(task.Id, task.Id, ExpirationMode.Absolute, expirationTimeout);
            var added = _runningTasks.Add(item);

            if (added == false)
                return false;

            try
            {
                var runningTask = task.Action.Invoke();
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
            return true;
        }
    }
}
