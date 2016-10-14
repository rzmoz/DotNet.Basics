using System;
using System.Collections.Generic;

namespace DotNet.Basics.Tasks
{
    public class TaskStartedEventArgs : EventArgs
    {
        public TaskStartedEventArgs(string taskName, IDictionary<string, string> taskAttributes)
        {
            TaskName = taskName;
            TaskProperties = new Dictionary<string, string>(taskAttributes);
        }

        public string TaskName { get; }
        public IReadOnlyDictionary<string, string> TaskProperties { get; }
    }
}
