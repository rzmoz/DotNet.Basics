namespace DotNet.Basics.Tasks
{
    public interface ITask
    {
        event ManagedTask.TaskLogEventHandler EntryLogged;
        event ManagedTask.TaskStartedEventHandler Started;
        event ManagedTask.TaskEndedEventHandler Ended;

        string Name { get; }
    }
}
