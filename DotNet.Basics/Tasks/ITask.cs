using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.Tasks
{
    public interface ITask : IHasLogging
    {
        string Name { get; }
        event ManagedTask.TaskStartedEventHandler Started;
        event ManagedTask.TaskEndedEventHandler Ended;
    }
}
