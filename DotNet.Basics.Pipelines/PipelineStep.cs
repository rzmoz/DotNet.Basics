using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.Pipelines
{
    public abstract class PipelineStep<T>(string? name) : ManagedTask<T>(name)
    {
        protected PipelineStep() : this(null) { }

        protected override Task<int> InnerRunAsync(T args, CancellationToken cancellationToken = default)
        {
            return RunImpAsync(args, cancellationToken);
        }

        protected abstract Task<int> RunImpAsync(T args, CancellationToken cancellationToken = default);
    }
}
