using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Collections;


namespace DotNet.Basics.Tasks
{
    public interface ITask : ITask<EventArgs>
    {
    }

    public interface ITask<T> where T : EventArgs, new()
    {
        event ManagedTask<T>.TaskStartedEventHandler TaskStarted;
        event ManagedTask<T>.TaskEndedEventHandler TaskEnded;

        string Name { get; }
        StringDictionary Properties { get; }

        void Init();
        Task<T> RunAsync();
        Task<T> RunAsync(CancellationToken ct);
        Task<T> RunAsync(T args, CancellationToken ct);
    }
}
