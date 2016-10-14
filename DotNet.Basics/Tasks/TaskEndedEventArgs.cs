using System;
using System.Collections.Generic;

namespace DotNet.Basics.Tasks
{
    public class TaskEndedEventArgs : TaskStartedEventArgs
    {
        public TaskEndedEventArgs(string taskName, IDictionary<string, string> taskAttributes, bool wasCancelled, Exception exception) : base(taskName, taskAttributes)
        {
            WasCancelled = wasCancelled;
            Exception = exception;
        }

        public bool WasCancelled { get; }
        public Exception Exception { get; }
    }
}
