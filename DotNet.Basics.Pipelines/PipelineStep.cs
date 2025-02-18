using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.Pipelines
{
    public abstract class PipelineStep<T>(string name) : ManagedTask<T>(name, "Step")
    {
        protected PipelineStep() : this(null)
        { }

        protected override Task<int> InnerRunAsync(T args, CancellationToken ct)
        {
            return RunImpAsync(args, ct);
        }

        protected abstract Task<int> RunImpAsync(T args, CancellationToken ct);
    }
}
