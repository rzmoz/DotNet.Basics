using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class SingletonTask : ManagedTask
    {
        private static readonly ConcurrentDictionary<string, string> _singletonScheduler = new ConcurrentDictionary<string, string>();
        
        public SingletonTask(Action task, string id = null) : base(task, id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException(nameof(id));
        }

        public SingletonTask(Func<CancellationToken, Task> task, string id = null) : base(task, id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException(nameof(id));
        }

        public SingletonTask(Action syncTask, Func<CancellationToken, Task> asyncTask, string id) : base(syncTask, asyncTask, id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException(nameof(id));
        }

        public bool IsRunning()
        {
            return _singletonScheduler.ContainsKey(Id);
        }

        public override void Run()
        {
            var runId = GetNewRunId();

            //try to lock task for running
            var added = _singletonScheduler.TryAdd(Id, runId);
            if (added == false)
            {
                FireTaskStarting(Id, null, false, "Task is already running");
                return;
            }

            try
            {
                FireTaskStarting(Id, runId, true, "Task is starting");
                SyncTask();
                FireTaskEnded(Id, runId, null);
            }
            catch (Exception e)
            {
                FireTaskEnded(Id, runId, e);
                throw;
            }
            finally
            {
                //make sure to unregister task when it's not running anymore
                _singletonScheduler.TryRemove(Id, out runId);
            }
        }

        public override async Task RunAsync(CancellationToken ct = new CancellationToken())
        {
            var runId = GetNewRunId();

            //try to lock task for running
            var added = _singletonScheduler.TryAdd(Id, runId);
            if (added == false)
            {
                FireTaskStarting(Id, null, false, "Task is already running");
                return;
            }

            try
            {
                FireTaskStarting(Id, runId, true, "Task is starting");
                await AsyncTask(ct).ConfigureAwait(false);
                FireTaskEnded(Id, runId, null);
            }
            catch (Exception e)
            {
                FireTaskEnded(Id, runId, e);
                throw;
            }
            finally
            {
                //make sure to unregister task when it's not running anymore
                _singletonScheduler.TryRemove(Id, out runId);
            }
        }
    }
}
