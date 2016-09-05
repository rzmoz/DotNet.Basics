using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class BackgroundTask : ManagedTask
    {
        public BackgroundTask(Action task, string id = null) : base(task, id)
        {
        }

        public BackgroundTask(Func<CancellationToken, Task> task, string id = null) : base(task, id)
        {
        }

        public BackgroundTask(Action syncTask, Func<CancellationToken, Task> asyncTask, string id) : base(syncTask, asyncTask, id)
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
