using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class AsyncTask : AsyncTask<TaskOptions>
    {
        public AsyncTask(Func<CancellationToken, Task> task, TaskOptions options = null) : base(task, options)
        {
        }
    }
    public class AsyncTask<T> : RunTask<T> where T : TaskOptions, new()
    {
        private readonly Func<CancellationToken, Task> _asyncTask;

        public AsyncTask(Func<CancellationToken, Task> task, T options = default(T)) : base(options)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            _asyncTask = task;
        }

        public override void Run()
        {
            throw new NotSupportedException($"Your're trying to run an async task synchronously. Use RunAsync");
        }

        public override async Task RunAsync(CancellationToken ct = new CancellationToken())
        {
            await _asyncTask.Invoke(ct).ConfigureAwait(false);
        }
    }
}
