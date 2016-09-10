using System;
using System.Collections.Concurrent;

namespace DotNet.Basics.Tasks
{
    public class SingletonTaskScheduler : TaskScheduler
    {
        private static readonly ConcurrentDictionary<string, bool> _singletonScheduler = new ConcurrentDictionary<string, bool>();

        public override bool IsRunning(string taskId)
        {
            return _singletonScheduler.ContainsKey(taskId);
        }

        protected override bool InnerTryAcquireStartTaskLock(string taskId)
        {
            if (taskId == null) throw new ArgumentNullException(nameof(taskId));
            if (string.IsNullOrWhiteSpace(taskId)) throw new ArgumentException(taskId);
            return _singletonScheduler.TryAdd(taskId, false);
        }

        protected override bool TryRemoveAcquiredTaskLock(string taskId)
        {
            if (taskId == null) throw new ArgumentNullException(nameof(taskId));
            if (string.IsNullOrWhiteSpace(taskId)) throw new ArgumentException(taskId);
            bool outVoid;
            return _singletonScheduler.TryRemove(taskId, out outVoid);
        }
    }
}
