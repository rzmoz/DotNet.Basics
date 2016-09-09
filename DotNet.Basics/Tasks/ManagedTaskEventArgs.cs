using System;

namespace DotNet.Basics.Tasks
{
    public class ManagedTaskEventArgs : EventArgs
    {
        public ManagedTaskEventArgs(string taskId, string runId)
        {
            TaskId = taskId;
            RunId = runId;
        }

        public string TaskId { get; }
        public string RunId { get; }
    }
}
