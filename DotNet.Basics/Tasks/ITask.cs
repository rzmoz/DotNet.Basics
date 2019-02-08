namespace DotNet.Basics.Tasks
{
    public interface ITask
    {
        string Name { get; }
        event ManagedTask.TaskStartedEventHandler Started;
        event ManagedTask.TaskEndedEventHandler Ended;
    }
}
