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

        protected ManagedTask(string name = null, params string[] removeSuffixes)
        {
            Name = name ?? GetType().GetNameWithGenericsExpanded();
            foreach (var removeSuffix in removeSuffixes)
                Name = Name.RemoveSuffix(removeSuffix);
        }

        public string Name { get; }

        public abstract Task RunAsync(object args);

        protected virtual void FireStarted(string taskName)
        {
            Started?.Invoke(taskName);
        }

        protected virtual void FireEnded(string taskName, Exception e = null)
        {
            Ended?.Invoke(taskName, e);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ManagedTask<T> : ManagedTask
    {
        private readonly Func<T, CancellationToken, Task> _task;

        public ManagedTask(string name, params string[] removeSuffixes)
            : this(name, (args, lct) => { }, removeSuffixes)
        { }

        public ManagedTask(Action syncTask, params string[] removeSuffixes)
            : this((args, ct) => syncTask(), removeSuffixes)
        { }

        public ManagedTask(Func<Task> asyncTask, params string[] removeSuffixes)
            : this((args, ct) => asyncTask(), removeSuffixes)
        { }

        public ManagedTask(Action<T, CancellationToken> task, params string[] removeSuffixes)
            : this(null, task, removeSuffixes)
        { }

        public ManagedTask(Func<T, CancellationToken, Task> task, params string[] removeSuffixes)
            : this(null, task, removeSuffixes)
        { }

        public ManagedTask(string name, Action<T, CancellationToken> task, params string[] removeSuffixes)
            : this(name, (args, ct) =>
            {
                task?.Invoke(args, ct);
                return Task.FromResult(string.Empty);
            }, removeSuffixes)
        { }

        public ManagedTask(string name, Func<T, CancellationToken, Task> task, params string[] removeSuffixes)
        : base(name, removeSuffixes)
        {
            _task = task ?? throw new ArgumentNullException(nameof(task));
        }

        public override async Task RunAsync(object args)
        {
            await RunAsync((T)args);
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
            return _task?.Invoke(args, ct);
        }
    }
}