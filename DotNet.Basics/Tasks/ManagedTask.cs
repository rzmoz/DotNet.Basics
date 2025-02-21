using System;
using System.Threading.Tasks;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tasks
{
    public abstract class ManagedTask : ITask
    {
        public delegate void TaskStartedEventHandler(string taskName);
        public delegate void TaskEndedEventHandler(string taskName, Exception? e);

        public event TaskStartedEventHandler? Started;
        public event TaskEndedEventHandler? Ended;

        protected ManagedTask(string? name = null)
        {
            Name = (name ?? GetType().GetNameWithGenericsExpanded()).ToTitleCase();
        }

        public string Name { get; }

        public abstract Task<int> RunAsync(object args);

        protected virtual void FireStarted(string taskName)
        {
            Started?.Invoke(taskName);
        }

        protected virtual void FireEnded(string taskName, Exception? e = null)
        {
            Ended?.Invoke(taskName, e);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ManagedTask<T>(string? name, Func<T, Task<int>> task) : ManagedTask(name)
    {
        private readonly Func<T, Task<int>> _task = task ?? throw new ArgumentNullException(nameof(task));

        public ManagedTask(Action task)
            : this(null, _ => task())
        { }
        public ManagedTask(Action<T> task)
            : this(null, task)
        { }
        public ManagedTask(Func<Task<int>> task)
            : this(null, _ => task())
        { }
        public ManagedTask(Func<T, Task<int>> task)
            : this(null, task)
        { }

        public ManagedTask(string? name)
            : this(name, _ => { })
        { }

        public ManagedTask(string? name, Action<T> task)
            : this(name, args =>
            {
                task?.Invoke(args);
                return Task.FromResult(0);
            })
        { }

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
        protected virtual async Task<int> InnerRunAsync(T args)
        {
            return await _task.Invoke(args);
        }
    }
}