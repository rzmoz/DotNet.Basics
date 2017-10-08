using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Tasks
{
    public class TaskResult
    {
        public TaskResult()
            : this(null, new List<TaskIssue>())
        { }
        public TaskResult(string taskName)
            : this(taskName, new List<TaskIssue>())
        { }

        public TaskResult(Action<TaskIssueList> addIssues)
            : this(null, ConstructList(addIssues))
        { }

        public TaskResult(string taskName, Action<TaskIssueList> addIssues)
            : this(taskName, ConstructList(addIssues))
        { }

        public TaskResult(string taskName, IEnumerable<TaskIssue> issues)
        {
            Name = taskName ?? string.Empty;
            Issues = issues?.ToList() ?? new List<TaskIssue>();
            Exceptions = Issues.Where(i => i.Exception != null).Select(i => i.Exception).ToList();
        }

        public string Name { get; }
        public IReadOnlyCollection<TaskIssue> Issues { get; }
        public IReadOnlyCollection<Exception> Exceptions { get; }

        public TaskResult Append(Action<TaskIssueList> addIssues)
        {
            return new TaskResult(Name, issues =>
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
