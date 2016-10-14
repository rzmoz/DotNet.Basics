using System;
using System.Collections.Generic;

namespace DotNet.Basics.Tasks
{
    public class TaskEndedEventArgs : TaskStartedEventArgs
    {
        public TaskEndedEventArgs(string name, string taskType, IReadOnlyDictionary<string, string> taskProperties, bool wasCancelled, Exception exception) : base(name, taskType, taskProperties)
        {
            WasCancelled = wasCancelled;
            Exception = exception;
        }

        public bool WasCancelled { get; }
        public Exception Exception { get; }
    }
}
