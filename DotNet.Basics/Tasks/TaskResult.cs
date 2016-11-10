using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Tasks
{
    public class TaskResult : TaskResult<EventArgs>
    {
        public TaskResult()
        { }

        public TaskResult(EventArgs args) : base(args)
        { }

        public TaskResult(Action<TaskIssueList> addIssues) : base(addIssues)
        { }

        public TaskResult(TaskIssueList issues) : base(issues)
        { }

        public TaskResult(EventArgs args, Action<TaskIssueList> addIssues) : base(args, addIssues)
        { }

        public TaskResult(EventArgs args, TaskIssueList issues) : base(args, issues)
        { }
    }

    public class TaskResult<T> where T : class, new()
    {
        public TaskResult()
            : this(null, new TaskIssueList())
        { }

        public TaskResult(T args)
            : this(args, issues => { })
        { }

        public TaskResult(Action<TaskIssueList> addIssues)
            : this(null, addIssues)
        { }

        public TaskResult(TaskIssueList issues)
            : this(null, issues)
        { }

        public TaskResult(T args, Action<TaskIssueList> addIssues)
            : this(args, ConstructList(addIssues))
        { }

        public TaskResult(T args, TaskIssueList issues)
        {
            Args = args ?? new T();
            Issues = (issues ?? new TaskIssueList()).ToList();
            NoIssues = Issues.Count == 0;
        }

        public T Args { get; }
        public IReadOnlyCollection<TaskIssue> Issues { get; }
        public bool NoIssues { get; }

        public TaskResult<T> Append(Action<TaskIssueList> addIssues)
        {
            return new TaskResult<T>(Args, issues =>
             {
                 issues.Add(Issues);
                 addIssues?.Invoke(issues);
             });
        }

        private static TaskIssueList ConstructList(Action<TaskIssueList> addIssues)
        {
            var issues = new TaskIssueList();
            addIssues?.Invoke(issues);
            return issues;
        }
    }
}
