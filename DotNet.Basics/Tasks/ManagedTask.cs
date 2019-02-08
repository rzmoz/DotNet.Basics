using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tasks
{
    public abstract class ManagedTask : ITask
    {
        public delegate void TaskStartedEventHandler(string taskName);
        public delegate void TaskEndedEventHandler(string taskName, Exception e);

        public event TaskStartedEventHandler Started;
        public event TaskEndedEventHandler Ended;

        protected ManagedTask(string name = null)
        {
            Name = name ?? GetType().GetNameWithGenericsExpanded();
        }

        public string Name { get; }

        protected virtual void FireStarted(string taskName)
        {
            Started?.Invoke(taskName);
        }

        protected virtual void FireEnded(string taskName, Exception e = null)
        {
            Ended?.Invoke(taskName, e);
        }
    }

    public class ManagedTask<T> : ManagedTask
    {
        private readonly Func<T, CancellationToken, Task> _task;

        public ManagedTask(string name) : this(name, (args, ct) => null)
        { }

        public ManagedTask(Action syncTask) : this((args, ct) => syncTask())
        { }

        public ManagedTask(Func<Task> asyncTask) : this((args, ct) => asyncTask())
        { }

        public ManagedTask(Action<T, CancellationToken> task)
            : this(null, task)
        { }

        public ManagedTask(Func<T, CancellationToken, Task> task)
            : this(null, task)
        { }

        public ManagedTask(string name, Action<T, CancellationToken> task)
            : this(name, (args, ct) =>
            {
                task?.Invoke(args, ct);
                return Task.FromResult(string.Empty);
            })
        { }

        public ManagedTask(string name, Func<T, CancellationToken, Task> task)
        : base(name)
        {
            _task = task ?? throw new ArgumentNullException(nameof(task));
            
        }

        public Task<T> RunAsync(T args)
        {
            return RunAsync(args, CancellationToken.None);
        }

        public async Task<T> RunAsync(T args, CancellationToken ct)
        {
            FireStarted(Name);
            try
            {
                if (ct.IsCancellationRequested == false)
                    await InnerRunAsync(args, ct).ConfigureAwait(false);

                FireEnded(Name);
                return args;
            }
            catch (Exception e)
            {
                FireEnded(Name, e);
                throw;
            }
        }

        protected virtual Task InnerRunAsync(T args, CancellationToken ct)
        {
            return _task(args, ct);
        }
    }
}