using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Collections;

namespace DotNet.Basics.Tasks
{
    public class ManagedTask<T> : ITask<T> where T : class, new()
    {
        private readonly Func<T, TaskIssueList, CancellationToken, Task> _task;
        private string _name;

        public delegate void TaskStartedEventHandler(TaskStartedEventArgs args);
        public delegate void TaskEndedEventHandler(TaskEndedEventArgs args);

        public ManagedTask(string name) : this(name, (args, issues, ct) => { })
        { }

        public ManagedTask(Func<Task> task) : this((args, issues, ct) => task())
        { }

        public ManagedTask(Action task) : this((args, issues, ct) => task())
        { }

        public ManagedTask(Action<T, TaskIssueList, CancellationToken> task)
            : this(null, task)
        { }

        public ManagedTask(Func<T, TaskIssueList, CancellationToken, Task> task)
            : this(null, task)
        { }

        public ManagedTask(string name, Action<T, TaskIssueList, CancellationToken> task)
            : this(name, (args, issues, ct) =>
             {
                 task?.Invoke(args, issues, ct);
                 return Task.FromResult("");
             })
        { }

        public ManagedTask(string name, Func<T, TaskIssueList, CancellationToken, Task> task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            _task = task;
            Name = name;
            Properties = new StringDictionary(DictionaryKeyMode.IgnoreKeyCase);
        }

        public event TaskStartedEventHandler Started;
        public event TaskEndedEventHandler Ended;

        public string Name
        {
            get { return _name; }
            set { _name = value ?? GetType().Name; }
        }

        public StringDictionary Properties { get; }

        public Task<TaskResult<T>> RunAsync()
        {
            return RunAsync(CancellationToken.None);
        }

        public Task<TaskResult<T>> RunAsync(CancellationToken ct)
        {
            return RunAsync(default(T), ct);
        }

        public async Task<TaskResult<T>> RunAsync(T args, CancellationToken ct)
        {
            Exception exceptionEncountered = null;
            if (args == null)
                args = new T();
            try
            {
                FireStarted(new TaskStartedEventArgs(Name, Properties));
                var issues = new TaskIssueList();
                await InnerRunAsync(args, issues, ct).ConfigureAwait(false);
                return new TaskResult<T>(args, issues);
            }
            catch (Exception e)
            {
                exceptionEncountered = e;
                throw;
            }
            finally
            {
                FireEnded(new TaskEndedEventArgs(Name, Properties, ct.IsCancellationRequested, exceptionEncountered));
            }
        }

        protected virtual async Task InnerRunAsync(T args, TaskIssueList issues, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                return;
            await _task(args, issues, ct).ConfigureAwait(false);
        }

        protected void FireStarted(TaskStartedEventArgs args)
        {
            Started?.Invoke(args);
        }
        protected void FireEnded(TaskEndedEventArgs args)
        {
            Ended?.Invoke(args);
        }
    }
}