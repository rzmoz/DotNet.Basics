using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.Tasks.Pipelines
{
    public abstract class PipelineStep<T> : ManagedTask<T> where T : class, new()
    {
        protected PipelineStep() : this(null)
        { }

        protected PipelineStep(string name) : base(name)
        { }

        protected override Task InnerRunAsync(T args, ConcurrentLog log, CancellationToken ct)
        {
            return RunImpAsync(args, log, ct);
        }

        protected abstract Task RunImpAsync(T args, ConcurrentLog log, CancellationToken ct);
    }
}
