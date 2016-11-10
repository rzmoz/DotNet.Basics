using System;
using System.Collections.Generic;

namespace DotNet.Basics.Tasks
{
    public class TaskIssueList : List<TaskIssue>
    {
        public TaskIssueList()
        {
        }

        public TaskIssueList(int capacity) : base(capacity)
        {
        }

        public TaskIssueList(IEnumerable<TaskIssue> collection) : base(collection)
        {
        }

        public void Add(string message)
        {
            Add(new TaskIssue(message));
        }
        public void Add(Exception e)
        {
            Add(new TaskIssue(e));
        }
        public void Add(string message, Exception e)
        {
            Add(new TaskIssue(message, e));
        }
    }
}
