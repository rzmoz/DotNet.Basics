using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Tasks
{
    public class TaskResultBuilder
    {
        private readonly ConcurrentQueue<TaskIssue> _issues;

        public TaskResultBuilder(string taskName = null)
            : this(taskName, null)
        {
        }

        public TaskResultBuilder(IEnumerable<TaskIssue> issues)
            : this(null, issues)
        {
        }
        public TaskResultBuilder(string taskName, IEnumerable<TaskIssue> issues)
        {
            TaskName = taskName ?? string.Empty;
            _issues = new ConcurrentQueue<TaskIssue>(issues ?? Enumerable.Empty<TaskIssue>());

        }

        public string TaskName { get; }

        public void AddRange(IEnumerable<TaskIssue> issues)
        {
            foreach (var issue in issues)
                Add(issue);
        }

        public TaskResultBuilder Add(string message)
        {
            _issues.Enqueue(new TaskIssue(message));
            return this;
        }

        public TaskResultBuilder Add(Exception e)
        {
            _issues.Enqueue(new TaskIssue(null, e));
            return this;
        }

        public TaskResultBuilder Add(string message, Exception e)
        {
            _issues.Enqueue(new TaskIssue(message, e));
            return this;
        }

        public TaskResultBuilder Add(TaskIssue issue)
        {
            _issues.Enqueue(issue);
            return this;
        }

        public TaskResult Build()
        {
            return new TaskResult(TaskName, _issues);
        }
    }
}
