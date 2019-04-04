using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.Pipelines
{
    public abstract class PipelineStep<T> : ManagedTask<T>
    {
        protected PipelineStep() : this(null)
        { }

        protected PipelineStep(string name) : base(name, "Step")
        { }

        protected override Task InnerRunAsync(T args, ILogDispatcher log, CancellationToken ct)
        {
            return RunImpAsync(args, log, ct);
        }

        protected abstract Task RunImpAsync(T args, ILogDispatcher log, CancellationToken ct);
    }
}
