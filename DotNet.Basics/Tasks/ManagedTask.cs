using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;
using Microsoft.Extensions.Logging;
using ILogger = DotNet.Basics.Diagnostics.ILogger;

namespace DotNet.Basics.Tasks
{
    public abstract class ManagedTask : ITask, ILogger
    {
        public delegate void TaskStartedEventHandler(string taskName);
        public delegate void TaskEndedEventHandler(string taskName, Exception e);

        public event TaskStartedEventHandler Started;
        public event TaskEndedEventHandler Ended;
        public event LogDispatcher.MessageLoggedEventHandler MessageLogged;

        protected ManagedTask(string name = null)
        {
            Name = name ?? GetType().GetNameWithGenericsExpanded();
        }

        public string Name { get; }


        protected virtual void FireMessageLogged(LogLevel level, string message, Exception e)
        {
            MessageLogged?.Invoke(level, message, e);
        }

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
        private readonly Func<T, ILogDispatcher, CancellationToken, Task> _task;
        private readonly ILogDispatcher _log;

        public ManagedTask(string name) : this(name, (args, log, ct) => { })
        { }

        public ManagedTask(Action syncTask) : this((args, log, ct) => syncTask())
        { }

        public ManagedTask(Func<Task> asyncTask) : this((args, log, ct) => asyncTask())
        { }

        public ManagedTask(Action<T, ILogDispatcher, CancellationToken> task)
            : this(null, task)
        { }

        public ManagedTask(Func<T, ILogDispatcher, CancellationToken, Task> task)
            : this(null, task)
        { }

        public ManagedTask(string name, Action<T, ILogDispatcher, CancellationToken> task)
            : this(name, (args, log, ct) =>
            {
                task?.Invoke(args, log, ct);
                return Task.FromResult(string.Empty);
            })
        { }

        public ManagedTask(string name, Func<T, ILogDispatcher, CancellationToken, Task> task)
        : base(name)
        {
            _task = task ?? throw new ArgumentNullException(nameof(task));
            _log = new LogDispatcher().InContext(Name);
            _log.MessageLogged += base.FireMessageLogged;
        }

        public Task<T> RunAsync(T args)
        {
            return RunAsync(args, default);
        }

        public async Task<T> RunAsync(T args, CancellationToken ct)
        {
            FireStarted(Name);
            try
            {
                if (ct.IsCancellationRequested == false)
                    await InnerRunAsync(args, _log, ct).ConfigureAwait(false);

                FireEnded(Name);
                return args;
            }
            catch (Exception e)
            {
                FireEnded(Name, e);
                throw;
            }
        }

        protected override void FireMessageLogged(LogLevel level, string message, Exception e)
        {
            _log.Write(level, message, e);
        }

        protected virtual Task InnerRunAsync(T args, ILogDispatcher log, CancellationToken ct)
        {
            return _task?.Invoke(args, log, ct);
        }
    }
}