using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Tasks
{
    public class TaskIssueList : ConcurrentQueue<TaskIssue>, IReadOnlyCollection<TaskIssue>
    {
        public TaskIssueList()
        { }

        public TaskIssueList(IEnumerable<TaskIssue> issues) : base(issues)
        { }

        public void Add(params TaskIssue[] issues)
        {
            foreach (var issue in issues)
                Enqueue(issue);
        }
        public void Add(IEnumerable<TaskIssue> issues)
        {
            if (issues == null)
                return;
            Add(issues.ToArray());
        }

        public void Add(string message)
        {
            Enqueue(new TaskIssue(message));
        }
        public void Add(Exception e)
        {
            Enqueue(new TaskIssue(e));
        }
        public void Add(string message, Exception e)
        {
            Enqueue(new TaskIssue(message, e));
        }
    }
}
