using System;
using System.Collections.Generic;

namespace DotNet.Basics.Tasks
{
    public class TaskStartedEventArgs<T> : EventArgs where T : EventArgs
    {
        public TaskStartedEventArgs(string taskName, IReadOnlyDictionary<string, string> taskAttributes, T args)
        {
            TaskName = taskName;
            TaskAttributes = taskAttributes;
            Args = args;
        }

        public string TaskName { get; }
        public IReadOnlyDictionary<string, string> TaskAttributes { get; }
        public T Args { get; }
    }
}
