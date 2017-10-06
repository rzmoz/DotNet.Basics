using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class ManagedTask<T> : ITask where T : class, new()
    {
        private readonly Func<T, TaskIssueList, CancellationToken, Task> _task;
        private string _name;

        public delegate void TaskEventHandler(TaskResult args);

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
        }

        public event TaskEventHandler Started;
        public event TaskEventHandler Ended;

        public string Name
        {
            get { return _name; }
            set { _name = value ?? GetType().Name; }
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
            var issues = new TaskIssueList();
            FireStarted(new TaskResult(Name));
            try
            {
                if (ct.IsCancellationRequested == false)
                    await InnerRunAsync(args, issues, ct).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                issues.Add(e);
                throw;
            }
            finally
            {
                FireEnded(new TaskResult(Name, issues));
            }
            return new TaskResult(Name, issues);
        }

        protected virtual async Task InnerRunAsync(T args, TaskIssueList issues, CancellationToken ct)
        {
            await _task(args, issues, ct).ConfigureAwait(false);
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