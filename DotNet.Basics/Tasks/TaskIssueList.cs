using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Tasks
{
    public class TaskIssueList : ConcurrentQueue<TaskIssue>, IReadOnlyCollection<TaskIssue>
    {
        public TaskIssueList()
        { }

        public TaskIssueList(IEnumerable<TaskIssue> collection) : base(collection)
        { }

        public void AddRange(IEnumerable<TaskIssue> issues)
        {
            foreach (var issue in issues)
                Add(issue);
        }

        public void Add(LogLevel logLevel, string message)
        {
            Enqueue(new TaskIssue(logLevel, message));
        }

        public void Add(LogLevel logLevel, Exception e)
        {
            Enqueue(new TaskIssue(logLevel, e.Message, e));
        }

        public void Add(LogLevel logLevel, string message, Exception e)
        {
            Enqueue(new TaskIssue(logLevel, message, e));
        }

        public void Add(TaskIssue issue)
        {
            Enqueue(issue);
        }
    }
}
