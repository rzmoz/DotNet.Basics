using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public interface ITask
    {
        void Run();
        Task RunAsync(CancellationToken ct = default(CancellationToken));
    }
}
