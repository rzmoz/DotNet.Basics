using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class AsyncTask : AsyncTask<TaskOptions>
    {
        public AsyncTask(Func<CancellationToken, Task> task, TaskOptions options = null, string id = null) : base(task, options, id)
        {
        }
    }
    public class AsyncTask<T> : RunTask<T> where T : TaskOptions, new()
    {
        private readonly Func<CancellationToken, Task> _asyncTask;

        public AsyncTask(Func<CancellationToken, Task> task, T options = default(T), string id = null) : base(options, id)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            _asyncTask = task;
        }

        public override void Run()
        {
            throw new NotSupportedException($"Async tasks can't be run synchronously reliably. Id:{Id}");
        }

        public override async Task RunAsync(CancellationToken ct = new CancellationToken())
        {
            await _asyncTask.Invoke(ct).ConfigureAwait(false);
        }
    }
}
