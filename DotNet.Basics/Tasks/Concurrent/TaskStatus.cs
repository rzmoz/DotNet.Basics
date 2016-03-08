namespace DotNet.Basics.Tasks.Concurrent
{
    public class TaskStatus
    {
        public TaskStatus(TaskState taskState, ITaskInfo taskInfo)
        {
            TaskState = taskState;
            TaskInfo = taskInfo;
        }

        public TaskState TaskState { get; }
        public ITaskInfo TaskInfo { get; }

        public override string ToString()
        {
            return $"{TaskState}";
        }
    }
}
