using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public interface ITask : ITask<EventArgs>
    { }

    public interface ITask<T> where T : class, new()
    {
        event ManagedTask<T>.TaskEventHandler Started;
        event ManagedTask<T>.TaskEventHandler Ended;

        string Name { get; }
        
        Task<TaskResult<T>> RunAsync();
        Task<TaskResult<T>> RunAsync(CancellationToken ct);
        Task<TaskResult<T>> RunAsync(T args, CancellationToken ct);
    }
}
