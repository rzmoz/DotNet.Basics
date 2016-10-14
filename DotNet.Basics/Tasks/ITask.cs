using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public interface ITask : ITask<EventArgs>
    {
    }

    public interface ITask<T> where T : EventArgs, new()
    {
        string Id { get; }
        string DisplayName { get; }
        Task<T> RunAsync();
        Task<T> RunAsync(CancellationToken ct);
        Task<T> RunAsync(T args, CancellationToken ct);
    }
}
