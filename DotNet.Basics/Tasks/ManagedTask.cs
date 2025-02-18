using System;
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
        private readonly Func<T, Task<int>> _task;

        public ManagedTask(Action task, params string[] removeSuffixes)
            : this(null, a => task(), removeSuffixes)
        { }
        public ManagedTask(Action<T> task, params string[] removeSuffixes)
            : this(null, task, removeSuffixes)
        { }
        public ManagedTask(Func<Task<int>> task, params string[] removeSuffixes)
            : this(null, a => task(), removeSuffixes)
        { }
        public ManagedTask(Func<T, Task<int>> task, params string[] removeSuffixes)
            : this(null, task, removeSuffixes)
        { }

        public ManagedTask(string name, params string[] removeSuffixes)
            : this(name, (args) => { }, removeSuffixes)
        { }

        public ManagedTask(string name, Action<T> task, params string[] removeSuffixes)
            : this(name, args =>
            {
                task?.Invoke(args);
                return Task.FromResult(0);
            }, removeSuffixes)
        { }

        public ManagedTask(string name, Func<T, Task<int>> task, params string[] removeSuffixes) : base(name, removeSuffixes)
        {
            _task = task ?? throw new ArgumentNullException(nameof(task));
        }
        public override async Task<int> RunAsync(object args)
        {
            return await RunAsync((T)args);
        }


        public async Task<int> RunAsync(T args)
        {
            FireStarted(Name);
            try
            {
                var exitCode = await InnerRunAsync(args).ConfigureAwait(false);

                FireEnded(Name);
                return exitCode;
            }
            catch (Exception e)
            {
                FireEnded(Name, e);
                throw;
            }
        }
        protected virtual Task<int> InnerRunAsync(T args)
        {
            return _task?.Invoke(args);
        }
    }
}