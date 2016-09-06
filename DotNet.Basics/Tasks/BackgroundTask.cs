using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class BackgroundTask : ManagedTask
    {
        public BackgroundTask(Action task) : base(task)
        {
        }

        public BackgroundTask(string id, Action task) : base(id, task)
        {
        }

        public BackgroundTask(Func<CancellationToken, Task> task) : base(task)
        {
        }

        public BackgroundTask(string id, Func<CancellationToken, Task> task) : base(id, task)
        {
        }

        public BackgroundTask(Action syncTask, Func<CancellationToken, Task> asyncTask) : base(syncTask, asyncTask)
        {
        }

        public BackgroundTask(string id, Action syncTask, Func<CancellationToken, Task> asyncTask) : base(id, syncTask, asyncTask)
        {
        }

        public override void Run()
        {
            Task.Run(() =>
            {
                var runId = GetNewRunId();
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

            }, CancellationToken.None);
        }

        public override Task RunAsync(CancellationToken ct = new CancellationToken())
        {
            Task.Run(async () =>
            {
                var runId = GetNewRunId();
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

            }, CancellationToken.None);
            return Task.CompletedTask;
        }
    }
}
