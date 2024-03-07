using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.Pipelines
{
    public abstract class PipelineStep<T> : ManagedTask<T>
    {
        protected PipelineStep() : this(null)
        { }

        protected PipelineStep(string name) : base(name, "Step")
        { }

        protected override Task InnerRunAsync(T args, CancellationToken ct)
        {
            return RunImpAsync(args, ct);
        }

        protected abstract Task RunImpAsync(T args, CancellationToken ct);
    }
}
