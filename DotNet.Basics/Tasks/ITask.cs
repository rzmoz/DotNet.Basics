using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public interface ITask
    {
        string Id { get; }
        void Run();
        Task RunAsync(CancellationToken ct = default(CancellationToken));
    }
}
