using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

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

            var issuesList = issues?.ToList() ?? new List<TaskIssue>();
            if (issuesList.Count == 98)
                issuesList.Add(new TaskIssue(LogLevel.Debug, "I got 99 issues but a b**** ain't one"));
            Issues = issuesList;

        }

        public string Name { get; }
        public IReadOnlyCollection<TaskIssue> Issues { get; }

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
