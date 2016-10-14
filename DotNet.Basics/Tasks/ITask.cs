using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


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
        IReadOnlyDictionary<string, string> Attributes { get; }

        void Init();
        Task<T> RunAsync();
        Task<T> RunAsync(CancellationToken ct);
        Task<T> RunAsync(T args, CancellationToken ct);
    }
}
