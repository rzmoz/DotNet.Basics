using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class BackgroundTask : ManagedTask
    {
        public BackgroundTask(ManagedTask task) : base(task)
        {
        }

        public BackgroundTask(string id, Action<string> syncTask) : base(id, syncTask)
        {
        }

        public BackgroundTask(string id, Func<string, Task> asyncTask) : base(id, asyncTask)
        {
        }

        internal override void Run(string runId)
        {
            Task.Run(() =>
            {
                base.Run(runId ?? string.Empty);
            }, CancellationToken.None);
        }

        internal override Task RunAsync(string runId)
        {
            Task.Run(async () =>
            {
                await base.RunAsync(runId ?? string.Empty).ConfigureAwait(false);
            }, CancellationToken.None);
            return Task.CompletedTask;
        }
    }
}
