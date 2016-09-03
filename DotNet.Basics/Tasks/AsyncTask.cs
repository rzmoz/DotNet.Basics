using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class AsyncTask : ITask
    {
        private readonly Func<CancellationToken, Task> _asyncTask;

        public AsyncTask(Func<CancellationToken, Task> task, string id = null)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            _asyncTask = task;
            Id = id ?? string.Empty;
        }

        public string Id { get; }

        public void Run()
        {
            throw new NotSupportedException($"Your're trying to run an async task synchronously. Use RunAsync");
        }

        public async Task RunAsync(CancellationToken ct = new CancellationToken())
        {
            await _asyncTask.Invoke(ct).ConfigureAwait(false);
        }
    }
}
