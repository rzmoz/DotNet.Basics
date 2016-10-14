using System;
using System.Collections.Generic;

namespace DotNet.Basics.Tasks
{
    public class TaskEndedEventArgs<T> : TaskStartedEventArgs<T> where T : EventArgs
    {
        public TaskEndedEventArgs(string taskName, IReadOnlyDictionary<string, string> taskAttributes, T args, bool wasCancelled, Exception exception) : base(taskName, taskAttributes, args)
        {
            WasCancelled = wasCancelled;
            Exception = exception;
        }

        public bool WasCancelled { get; }
        public Exception Exception { get; }
    }
}
