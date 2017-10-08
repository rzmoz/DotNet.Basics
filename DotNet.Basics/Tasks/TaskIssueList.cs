using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

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

        public void Add(string message)
        {
            Enqueue(new TaskIssue(message));
        }

        public void Add(Exception e)
        {
            Enqueue(new TaskIssue(null, e));
        }

        public void Add(string message, Exception e)
        {
            Enqueue(new TaskIssue(message, e));
        }

        public void Add(TaskIssue issue)
        {
            Enqueue(issue);
        }
    }
}
