using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Tasks
{
    public class ManagedTask<T> : ITask where T : class, new()
    {
        private readonly Func<T, CancellationToken, Task> _task;
        private string _name;

        public delegate void TaskLogEventHandler(string taskName, LogEntry entry);
        public delegate void TaskStartedEventHandler(string taskName);
        public delegate void TaskEndedEventHandler(string taskName, Exception e);

        public event TaskLogEventHandler EntryLogged;
        public event TaskStartedEventHandler Started;
        public event TaskEndedEventHandler Ended;

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
        {
            _task = task ?? throw new ArgumentNullException(nameof(task));
            Name = name;
        }

        public string Name
        {
            get => _name;
            set => _name = value ?? GetType().Name;
        }

        public Task<T> RunAsync()
        {
            return RunAsync(CancellationToken.None);
        }

        public Task<T> RunAsync(CancellationToken ct)
        {
            return RunAsync(default(T), ct);
        }

        public async Task<T> RunAsync(T args, CancellationToken ct)
        {
            if (args == null)
                args = new T();

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

        protected virtual async Task InnerRunAsync(T args, CancellationToken ct)
        {
            await _task(args, ct).ConfigureAwait(false);
        }

        protected void Log(LogLevel level, string message, Exception e = null)
        {
            Log(Name, new LogEntry(level, message, e));
        }

        protected void Log(string taskName, LogEntry entry)
        {
            EntryLogged?.Invoke(taskName, entry);
        }

        protected void FireStarted(string taskName)
        {
            Started?.Invoke(taskName);
            Log(LogLevel.Trace, $"Started: {Name} in ({GetType().FullName})");
        }

        protected void FireEnded(string taskName, Exception e = null)
        {
            Ended?.Invoke(taskName, e);
            if (e != null)
                Log(LogLevel.Error, e.Message, e);
            Log(LogLevel.Trace, $"Ended: {Name} in ({GetType().FullName}");
        }
    }
}