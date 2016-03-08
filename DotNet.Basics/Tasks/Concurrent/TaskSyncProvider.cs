using System;
using System.Runtime.Caching;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tasks.Concurrent
{
    public class TaskSyncProvider
    {
        private readonly MemoryCache _cache;

        public TaskSyncProvider()
            : this(15.Seconds())
        { }

        public TaskSyncProvider(TimeSpan lockTimeout)
        {
            LockTimeout = lockTimeout;
            DataTimeout = 30.Days();
            _cache = MemoryCache.Default;
        }

        public TimeSpan LockTimeout { get; }
        public TimeSpan DataTimeout { get; }

        public bool IsLocked(string taskName)
        {
            return _cache.Get(GetLockId(taskName)) != null;
        }

        public void SetLock(string taskName)
        {
            var lockSetTimestamp = DateTime.UtcNow;
            //renew lock - absolute expiration so peak from other threads (is locked) doesn't extend lease
            var timeout = lockSetTimestamp + LockTimeout;
            var cacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = timeout };
            _cache.Set(GetLockId(taskName), lockSetTimestamp.Ticks, cacheItemPolicy);
        }

        public void RemoveLock(string taskName)
        {
            _cache.Remove(GetLockId(taskName));
        }

        public void RemoveTaskInfo(string taskName)
        {
            _cache.Remove(GetInfoId(taskName));
        }

        public void SetTaskInfo(ITaskInfo taskInfo)
        {
            if (taskInfo == null) throw new ArgumentNullException(nameof(taskInfo));

            var cacheItemPolicy = new CacheItemPolicy { SlidingExpiration = DataTimeout };
            _cache.Set(taskInfo.Id, taskInfo, cacheItemPolicy);
        }

        public ITaskInfo GetTaskInfo(string taskName)
        {
            return _cache.Get(GetInfoId(taskName)) as ITaskInfo;
        }

        private string GetLockId(string taskName)
        {
            var id = new TaskInfo(taskName);
            return $"{id.Id}.lck";
        }

        private string GetInfoId(string taskName)
        {
            return new TaskId(taskName).Id;
        }
    }
}
