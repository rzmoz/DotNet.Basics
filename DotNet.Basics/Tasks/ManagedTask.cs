using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Tasks
{
    public class ManagedTask<T> : ITask where T : class, new()
    {
        private readonly Func<T, ConcurrentLog, CancellationToken, Task> _task;
        private string _name;

        public delegate void TaskEventHandler(TaskResult args);

        public ManagedTask(string name) : this(name, (args, log, ct) => { })
        { }

        public ManagedTask(Func<Task> task) : this((args, log, ct) => task())
        { }

        public ManagedTask(Action task) : this((args, log, ct) => task())
        { }

        public ManagedTask(Action<T, ConcurrentLog, CancellationToken> task)
            : this(null, task)
        { }

        public ManagedTask(Func<T, ConcurrentLog, CancellationToken, Task> task)
            : this(null, task)
        { }

        public ManagedTask(string name, Action<T, ConcurrentLog, CancellationToken> task)
            : this(name, (args, log, ct) =>
             {
                 task?.Invoke(args, log, ct);
                 return Task.FromResult(string.Empty);
             })
        { }

        public ManagedTask(string name, Func<T, ConcurrentLog, CancellationToken, Task> task)
        {
            _task = task ?? throw new ArgumentNullException(nameof(task));
            Name = name;
        }

        public event TaskEventHandler Started;
        public event TaskEventHandler Ended;

        public string Name
        {
            get => _name;
            set => _name = value ?? GetType().Name;
        }

        public Task<TaskResult> RunAsync()
        {
            return RunAsync(CancellationToken.None);
        }

        public Task<TaskResult> RunAsync(CancellationToken ct)
        {
            return RunAsync(default(T), ct);
        }

        public async Task<TaskResult> RunAsync(T args, CancellationToken ct)
        {
            if (args == null)
                args = new T();
            var log = new ConcurrentLog();
            FireStarted(new TaskResult(Name));
            try
            {
                if (ct.IsCancellationRequested == false)
                    await InnerRunAsync(args, log, ct).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                log.Add(LogLevel.Error, e);
                throw;
            }
            finally
            {
                FireEnded(new TaskResult(Name, log));
            }
            return new TaskResult(Name, log);
        }

        protected virtual async Task InnerRunAsync(T args, ConcurrentLog log, CancellationToken ct)
        {
            await _task(args, log, ct).ConfigureAwait(false);
        }

        protected void FireStarted(TaskResult args)
        {
            Started?.Invoke(args);
        }

        protected void FireEnded(TaskResult args)
        {
            Ended?.Invoke(args);
        }
    }
}