using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class OnceOnlyAsyncTask
    {
        private Func<CancellationToken, Task> _task;

        public OnceOnlyAsyncTask(Func<CancellationToken, Task> task)
        {
            _task = async (ct) =>
            {
                _task = ctx => Task.CompletedTask;
                await task.Invoke(ct).ConfigureAwait(false);
            };
        }

        public async Task RunAsync(CancellationToken ct = default(CancellationToken))
        {
            await _task.Invoke(ct).ConfigureAwait(false);
        }
    }
}
