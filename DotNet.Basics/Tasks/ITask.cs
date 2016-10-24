using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Collections;

namespace DotNet.Basics.Tasks
{
    public interface ITask : ITask<EventArgs>
    {
    }

    public interface ITask<T> where T : new()
    {
        event ManagedTask<T>.TaskStartedEventHandler Started;
        event ManagedTask<T>.TaskEndedEventHandler Ended;

        string Name { get; }
        StringDictionary Properties { get; }

        void Init();
        Task<T> RunAsync();
        Task<T> RunAsync(CancellationToken ct);
        Task<T> RunAsync(T args, CancellationToken ct);
    }
}
