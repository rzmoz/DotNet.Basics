namespace DotNet.Basics.Tasks
{
    public class AtMostOnceTaskRunResult
    {
        public AtMostOnceTaskRunResult(string taskId, bool started, string reason = null)
        {
            TaskId = taskId;
            Started = started;
            Reason = reason ?? string.Empty;
        }

        public string TaskId { get; }
        public bool Started { get; }
        public string Reason { get; }
    }
}
