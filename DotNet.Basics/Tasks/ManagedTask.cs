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

        public abstract Task<int> RunAsync(object args);
        public abstract Task<int> RunAsync(object args, CancellationToken ct);

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

    public class ManagedTask<T>(string name, Func<T, CancellationToken, Task<int>> task, params string[] removeSuffixes) : ManagedTask(name, removeSuffixes)
    {
        private readonly Func<T, CancellationToken, Task<int>> _task = task ?? throw new ArgumentNullException(nameof(task));

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

        public ManagedTask(Func<T, CancellationToken, Task<int>> task, params string[] removeSuffixes)
            : this(null, task, removeSuffixes)
        { }

        public ManagedTask(string name, Action<T, CancellationToken> task, params string[] removeSuffixes)
            : this(name, (args, ct) =>
            {
                task?.Invoke(args, ct);
                return Task.FromResult(0);
            }, removeSuffixes)
        { }

        public override async Task<int> RunAsync(object args)
        {
            return await RunAsync((T)args);
        }

        public override async Task<int> RunAsync(object args, CancellationToken ct)
        {
            return await RunAsync((T)args, ct);
        }

        public Task<int> RunAsync(T args)
        {
            return RunAsync(args, CancellationToken.None);
        }

        public async Task<int> RunAsync(T args, CancellationToken ct)
        {
            var exitCode = -1;
            FireStarted(Name);
            try
            {
                if (ct.IsCancellationRequested == false)
                    exitCode = await InnerRunAsync(args, ct).ConfigureAwait(false);

                FireEnded(Name);
                return exitCode;
            }
            catch (Exception e)
            {
                FireEnded(Name, e);
                throw;
            }
        }
        protected virtual Task<int> InnerRunAsync(T args, CancellationToken ct)
        {
            return _task?.Invoke(args, ct);
        }
    }
}