namespace DotNet.Basics.Tasks.Concurrent
{
    public interface IConcurrentTaskInfoReader
    {
        ITaskInfo GetTaskInfo(string taskName);
        TaskStatus GetStatus(string taskName);
    }
}
