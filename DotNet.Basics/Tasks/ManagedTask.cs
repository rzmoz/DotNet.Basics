using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tasks
{
    public class ManagedTask<T> : ITask, IHasLogging where T : class, new()
    {
        public delegate void TaskStartedEventHandler(string taskName);
        public delegate void TaskEndedEventHandler(string taskName, Exception e);

        public event LogEntry.TaskLogEventHandler EntryLogged;
        public event TaskStartedEventHandler Started;
        public event TaskEndedEventHandler Ended;

        private readonly Func<T, LoggingContext, CancellationToken, Task> _task;
        private string _name;

        protected LoggingContext Log { get; }

        public ManagedTask(string name) : this(name, (args, log, ct) => null)
        { }

        public ManagedTask(Action syncTask) : this((args, log, ct) => syncTask())
        { }

        public ManagedTask(Func<Task> asyncTask) : this((args, log, ct) => asyncTask())
        { }

        public ManagedTask(Action<T, LoggingContext, CancellationToken> task)
            : this(null, task)
        { }

        public ManagedTask(Func<T, LoggingContext, CancellationToken, Task> task)
            : this(null, task)
        { }

        public ManagedTask(string name, Action<T, LoggingContext, CancellationToken> task)
            : this(name, (args, log, ct) =>
            {
                task?.Invoke(args, log, ct);
                return Task.FromResult(string.Empty);
            })
        { }

        public ManagedTask(string name, Func<T, LoggingContext, CancellationToken, Task> task)
        {
            _task = task ?? throw new ArgumentNullException(nameof(task));
            Name = name;
            Log = new LoggingContext(Name);
            Log.EntryLogged += e => EntryLogged?.Invoke(e);
        }

        public string Name
        {
            get => _name;
            set => _name = value ?? GetType().GetNameWithGenericsExpanded();
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
            await _task(args, Log, ct).ConfigureAwait(false);
        }

        protected virtual void FireStarted(string taskName)
        {
            Started?.Invoke(taskName);
        }

        protected virtual void FireEnded(string taskName, Exception e = null)
        {
            Ended?.Invoke(taskName, e);
        }
    }
}