namespace DotNet.Basics.Tasks
{
    public class AtMostOnceTaskRunResult
    {
        public AtMostOnceTaskRunResult(bool started, string reason = null)
        {
            Started = started;
            Reason = reason ?? string.Empty;
        }

        public bool Started { get; }
        public string Reason { get; }
    }
}
