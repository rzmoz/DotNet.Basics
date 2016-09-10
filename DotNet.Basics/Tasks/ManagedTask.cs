using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class ManagedTask
    {
        private static readonly ConcurrentDictionary<string, ConcurrentStack<bool>> _taskScheduler =
            new ConcurrentDictionary<string, ConcurrentStack<bool>>();

        private readonly Action<string> _syncTask;
        private readonly Func<string, Task> _asyncTask;
        private readonly Func<bool> _isRunning;
        private readonly Func<string, bool> _tryAcquireStartlock;

        public delegate void ManagedTaskEventHandler(ManagedTaskEventArgs args);

        public delegate void ManagedTaskEndedEventHandler(ManagedTaskEndedEventArgs args);

        public event ManagedTaskEventHandler TaskStarted;
        public event ManagedTaskEndedEventHandler TaskEnded;

        public ManagedTask(ManagedTask task)
            : this(task.Id, task.Run, task.RunAsync, task.TryAcquireStartlock, task.IsRunning)
        {
        }

        public ManagedTask(string id, Action<string> syncTask, Func<string, bool> tryAcquireStartlock = null, Func<bool> isRunning = null)
            : this(id, syncTask, rid =>
            {
                syncTask.Invoke(rid);
                return Task.CompletedTask;
            }, tryAcquireStartlock, isRunning)
        {
        }

        public ManagedTask(string id, Func<string, Task> asyncTask, Func<string, bool> tryAcquireStartlock = null, Func<bool> isRunning = null)
            : this(id, rid => { asyncTask.Invoke(rid).Wait(); }, asyncTask, tryAcquireStartlock, isRunning)
        {
        }

        private ManagedTask(string id, Action<string> syncTask, Func<string, Task> asyncTask, Func<string, bool> tryAcquireStartlock, Func<bool> isRunning)
        {
            Id = id ?? string.Empty;
            _syncTask = syncTask ?? VoidSyncTask;
            _asyncTask = asyncTask ?? VoidAsyncTask;
            _tryAcquireStartlock = tryAcquireStartlock ?? DefaultTryAcquireStartTaskLock;
            _isRunning = isRunning ?? DefaultIsRunning;
        }

        public string Id { get; }

        public virtual bool IsRunning()
        {
            return _isRunning();
        }
        
        internal virtual bool TryAcquireStartlock(string runId)
        {
            return _tryAcquireStartlock(runId);
        }

        internal virtual void Run(string runId)
        {
            try
            {
                FireTaskStarted(runId);
                _syncTask(runId ?? string.Empty);
                FireTaskEnded(runId, TaskEndedReason.AllGood, null);
            }
            catch (Exception e)
            {
                var asAggrE = e;
                while (asAggrE is AggregateException && asAggrE.InnerException != null)
                    asAggrE = asAggrE.InnerException;

                FireTaskEnded(runId, TaskEndedReason.Exception, e);
                throw;
            }
            finally
            {
                try
                {
                    bool poppedTask;
                    var runStack = _taskScheduler[Id];
                    runStack.TryPop(out poppedTask);
                }
                catch (KeyNotFoundException)
                {
                    //ignore
                }
            }
        }

        internal virtual async Task RunAsync(string runId)
        {
            try
            {
                FireTaskStarted(runId);
                await _asyncTask(runId ?? string.Empty).ConfigureAwait(false);
                FireTaskEnded(runId, TaskEndedReason.AllGood, null);
            }
            catch (Exception e)
            {
                var asAggrE = e;
                while (asAggrE is AggregateException && asAggrE.InnerException != null)
                    asAggrE = asAggrE.InnerException;

                FireTaskEnded(runId, TaskEndedReason.Exception, e);
                throw;
            }
            finally
            {
                try
                {
                    bool poppedTask;
                    var runStack = _taskScheduler[Id];
                    runStack.TryPop(out poppedTask);
                }
                catch (KeyNotFoundException)
                {
                    //ignore
                }
            }
        }

        private bool DefaultIsRunning()
        {
            ConcurrentStack<bool> runStack;
            if (_taskScheduler.TryGetValue(Id, out runStack) == false)
                return false;
            return runStack.Count > 0;
        }
        private bool DefaultTryAcquireStartTaskLock(string runId)
        {
            _taskScheduler.TryAdd(Id, new ConcurrentStack<bool>());
            var runStack = _taskScheduler[Id];
            runStack.Push(false);
            return true;
        }

        private void FireTaskStarted(string runId)
        {
            TaskStarted?.Invoke(new ManagedTaskEventArgs(Id, runId));
        }

        private void FireTaskEnded(string runId, TaskEndedReason reason, Exception e)
        {
            TaskEnded?.Invoke(new ManagedTaskEndedEventArgs(Id, runId, reason, e));
        }

        private void VoidSyncTask(string runId)
        { }
        private Task VoidAsyncTask(string runId)
        {
            return Task.CompletedTask;
        }
    }
}