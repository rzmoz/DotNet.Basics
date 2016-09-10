using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class TransientTaskScheduler : TaskScheduler
    {
        private static readonly ConcurrentDictionary<string, ConcurrentStack<bool>> _taskScheduler = new ConcurrentDictionary<string, ConcurrentStack<bool>>();

        public override bool IsRunning(string taskId)
        {
            ConcurrentStack<bool> runStack;
            if (_taskScheduler.TryGetValue(taskId, out runStack) == false)
                return false;
            return runStack.Count > 0;
        }

        protected override bool TryAcquireStartTaskLock(string taskId)
        {
            _taskScheduler.TryAdd(taskId, new ConcurrentStack<bool>());
            var runStack = _taskScheduler[taskId];
            runStack.Push(false);
            return true;
        }

        protected override bool TryRemoveAcquiredTaskLock(string taskId)
        {
            try
            {
                bool poppedTask;
                var runStack = _taskScheduler[taskId];
                runStack.TryPop(out poppedTask);
                return true;
            }
            catch (KeyNotFoundException)
            {
                //ignore
                return false;
            }
        }
    }
}
