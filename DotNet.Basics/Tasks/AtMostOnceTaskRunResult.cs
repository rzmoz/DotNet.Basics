namespace DotNet.Basics.Tasks
{
    public class AtMostOnceTaskRunResult
    {
        public AtMostOnceTaskRunResult(string id, bool started, string reason = null)
        {
            Id = id;
            Started = started;
            Reason = reason ?? string.Empty;
        }

        public string Id { get; }
        public bool Started { get; }
        public string Reason { get; }
    }
}
