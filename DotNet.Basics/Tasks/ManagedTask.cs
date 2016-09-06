using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class ManagedTask
    {
        protected Action SyncTask { get; }
        protected Func<CancellationToken, Task> AsyncTask { get; }

        public delegate void TaskStartingEventHandler(string taskId, string runId, bool started, string reason);
        public delegate void TaskEndedEventHandler(string taskId, string runId, Exception lastException);

        public event TaskStartingEventHandler TaskStarting;
        public event TaskEndedEventHandler TaskEnded;


        public ManagedTask(Action task)
            : this(string.Empty, task)
        {
        }

        public ManagedTask(string id, Action task)
            : this(id, task, ct =>
             {
                 task.Invoke();
                 return Task.CompletedTask;
             })
        {
        }

        public ManagedTask(Func<CancellationToken, Task> task)
            : this(string.Empty, task)
        {
        }

        public ManagedTask(string id, Func<CancellationToken, Task> task)
            : this(id, () =>
              {
                  var asyncTask = task.Invoke(CancellationToken.None);
                  asyncTask.Wait();
              }, task)
        {
        }


        public ManagedTask(Action syncTask, Func<CancellationToken, Task> asyncTask)
            : this(null, syncTask, asyncTask)
        {
        }

        public ManagedTask(string id, Action syncTask, Func<CancellationToken, Task> asyncTask)
        {
            if (syncTask == null) throw new ArgumentNullException(nameof(syncTask));
            if (asyncTask == null) throw new ArgumentNullException(nameof(asyncTask));
            SyncTask = syncTask;
            AsyncTask = asyncTask;
            Id = id ?? string.Empty;
        }

        public string Id { get; }

        internal string GetNewRunId()
        {
            return Guid.NewGuid().ToString("N");
        }

        public virtual void Run()
        {
            var runId = GetNewRunId();

            try
            {
                FireTaskStarting(Id, runId, true, null);
                SyncTask();
                FireTaskEnded(Id, runId, null);
            }
            catch (Exception e)
            {
                FireTaskEnded(Id, runId, e);
                throw;
            }
        }

        public virtual async Task RunAsync(CancellationToken ct = default(CancellationToken))
        {
            var runId = GetNewRunId();

            try
            {
                FireTaskStarting(Id, runId, true, null);
                await AsyncTask(ct).ConfigureAwait(false);
                FireTaskEnded(Id, runId, null);
            }
            catch (Exception e)
            {
                FireTaskEnded(Id, runId, e);
                throw;
            }
        }

        protected void FireTaskStarting(string taskId, string runId, bool taskWillStart, string reason)
        {
            TaskStarting?.Invoke(taskId, runId, taskWillStart, reason);
        }

        protected void FireTaskEnded(string taskId, string runId, Exception lastException)
        {
            TaskEnded?.Invoke(taskId, runId, lastException);
        }
    }
}
