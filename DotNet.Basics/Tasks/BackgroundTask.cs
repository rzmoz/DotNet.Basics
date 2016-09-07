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

        public BackgroundTask(Action<string> task) : base(task)
        {
        }

        public BackgroundTask(string id, Action<string> task) : base(id, task)
        {
        }

        public BackgroundTask(Func<string, Task> task) : base(task)
        {
        }

        public BackgroundTask(string id, Func<string, Task> task) : base(id, task)
        {
        }
        
        internal override void Run(string runId = null)
        {
            Task.Run(() =>
            {
                base.Run(runId ?? string.Empty);
            }, CancellationToken.None);
        }

        internal override Task RunAsync(string runId = null)
        {
            Task.Run(async () =>
            {
                await base.RunAsync(runId ?? string.Empty).ConfigureAwait(false);
            }, CancellationToken.None);
            return Task.CompletedTask;
        }
    }
}
