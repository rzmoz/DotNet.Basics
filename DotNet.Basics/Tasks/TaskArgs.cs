using System;
using System.Collections.Generic;

namespace DotNet.Basics.Tasks
{
    public class TaskArgs
    {
        public TaskArgs(string name, bool wasCancelled, IReadOnlyCollection<TaskIssue> issues = null, Exception exception = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            Name = name;
            WasCancelled = wasCancelled;
            Issues = issues ?? new TaskIssue[0];
            Exception = exception;
        }

        public string Name { get; }
        public bool WasCancelled { get; }
        public IReadOnlyCollection<TaskIssue> Issues { get; }
        public Exception Exception { get; }
    }
}
