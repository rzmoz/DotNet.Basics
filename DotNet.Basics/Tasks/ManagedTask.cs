using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class ManagedTask<T> : ITask<T> where T : class, new()
    {
        private readonly Func<T, TaskIssueList, CancellationToken, Task> _task;
        private string _name;

        public delegate void TaskEventHandler(TaskArgs args);

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
            PreConditionsAsync = null;
            PostConditionsAsync = null;
        }

        public event TaskEventHandler Started;
        public event TaskEventHandler Ended;

        public string Name
        {
            get { return _name; }
            set { _name = value ?? GetType().Name; }
        }

        public Func<T, TaskIssueList, CancellationToken, Task<bool>> PreConditionsAsync { get; set; }
        public Func<T, TaskIssueList, CancellationToken, Task<bool>> PostConditionsAsync { get; set; }

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
            if (args == null)
                args = new T();
            Exception exceptionEncountered = null;
            var issues = new TaskIssueList();
            //var result = new TaskResult<T>(args, issues);
            FireStarted(new TaskArgs(Name, ct.IsCancellationRequested));
            try
            {
                var preconditionsMet = await InnerPreConditionsAsync(args, issues, ct).ConfigureAwait(false);
                if (preconditionsMet == false)
                    issues.Add("Pre-Conditions not met. Inspect issues for details");
                else if (ct.IsCancellationRequested == false)
                {
                    await InnerRunAsync(args, issues, ct).ConfigureAwait(false);

                    var postConditionsMet = await InnerPostConditionsAsync(args, issues, ct).ConfigureAwait(false);
                    if (postConditionsMet == false)
                        issues.Add("POST-Conditions not met. Inspect issues for details");
                }

                return new TaskResult<T>(args, issues);
            }
            catch (Exception e)
            {
                exceptionEncountered = e;
                throw;
            }
            finally
            {
                FireEnded(new TaskArgs(Name, ct.IsCancellationRequested, issues, exceptionEncountered));
            }
        }

        protected virtual Task<bool> InnerPreConditionsAsync(T args, TaskIssueList issues, CancellationToken ct)
        {
            return PreConditionsAsync?.Invoke(args, issues, ct) ?? Task.FromResult(true);
        }

        protected virtual Task<bool> InnerPostConditionsAsync(T args, TaskIssueList issues, CancellationToken ct)
        {
            return PostConditionsAsync?.Invoke(args, issues, ct) ?? Task.FromResult(true);
        }

        protected virtual async Task InnerRunAsync(T args, TaskIssueList issues, CancellationToken ct)
        {
            await _task(args, issues, ct).ConfigureAwait(false);
        }

        protected void FireStarted(TaskArgs args)
        {
            Started?.Invoke(args);
        }

        protected void FireEnded(TaskArgs args)
        {
            Ended?.Invoke(args);
        }
    }
}