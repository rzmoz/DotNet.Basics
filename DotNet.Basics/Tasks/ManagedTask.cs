using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class ManagedTask<T> : ITask<T> where T : EventArgs, new()
    {
        private readonly Func<T, CancellationToken, Task> _task;

        public string Id { get; }
        public string DisplayName { get; }

        public ManagedTask(Func<Task> task) : this((args, ct) => task())
        {
        }
        public ManagedTask(Action task) : this((args, ct) => task())
        {

        }

        public ManagedTask(Func<T, CancellationToken, Task> task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            _task = task;
        }
        public ManagedTask(Action<T, CancellationToken> task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            _task = (args, ct) =>
            {
                task(args, ct);
                return Task.CompletedTask;
            };
        }

        public Task<T> RunAsync()
        {
            return RunAsync(new T(), CancellationToken.None);
        }

        public Task<T> RunAsync(CancellationToken ct)
        {
            return RunAsync(new T(), ct);
        }

        public async Task<T> RunAsync(T args, CancellationToken ct)
        {
            await _task(args, ct).ConfigureAwait(false);
            return args;
        }
    }
}