using System;

namespace DotNet.Basics.Tasks
{
    public class ManagedTaskEndedEventArgs : ManagedTaskEventArgs
    {
        public ManagedTaskEndedEventArgs(string taskId, string runId, TaskEndedReason reason, Exception exception) : base(taskId, runId)
        {
            Reason = reason;
            Exception = exception;
        }

        public TaskEndedReason Reason { get; }
        public Exception Exception { get; }
    }
}
