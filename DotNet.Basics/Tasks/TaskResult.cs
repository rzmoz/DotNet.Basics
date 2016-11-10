using System;
using System.Collections.Generic;

namespace DotNet.Basics.Tasks
{
    public class TaskResult : TaskResult<EventArgs>
    {
        public TaskResult()
        {
        }

        public TaskResult(EventArgs args) : base(args)
        {
        }

        public TaskResult(Action<TaskIssueList> addIssues) : base(addIssues)
        {
        }

        public TaskResult(EventArgs args, Action<TaskIssueList> addIssues) : base(args, addIssues)
        {
        }
    }

    public class TaskResult<T> where T : new()
    {
        public TaskResult()
            : this(new T(), new TaskIssueList())
        { }

        public TaskResult(T args)
            : this(args, issues => { })
        { }

        public TaskResult(Action<TaskIssueList> addIssues)
            : this(new T(), addIssues)
        { }

        public TaskResult(TaskIssueList issues)
            : this(new T(), issues)
        { }

        public TaskResult(T args, Action<TaskIssueList> addIssues)
            : this(args, ConstructList(addIssues))
        { }

        public TaskResult(T args, TaskIssueList issues)
        {
            Args = args;
            Issues = issues ?? new TaskIssueList();
            NoIssues = Issues.Count == 0;
        }

        public T Args { get; }
        public IReadOnlyCollection<TaskIssue> Issues { get; }
        public bool NoIssues { get; }

        public TaskResult<T> Append(Action<TaskIssueList> addIssues)
        {
            return new TaskResult<T>(Args, issues =>
             {
                 issues.AddRange(Issues);
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
