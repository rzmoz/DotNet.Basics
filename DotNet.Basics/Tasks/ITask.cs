using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Collections;

namespace DotNet.Basics.Tasks
{
    public interface ITask : ITask<EventArgs>
    {
    }

    public interface ITask<T> where T : class, new()
    {
        event ManagedTask<T>.TaskStartedEventHandler Started;
        event ManagedTask<T>.TaskEndedEventHandler Ended;

        string Name { get; }
        StringDictionary Properties { get; }

        void Init();

        Task<TaskResult<T>> RunAsync();
        Task<TaskResult<T>> RunAsync(CancellationToken ct);
        Task<TaskResult<T>> RunAsync(T args, CancellationToken ct);
    }
}
