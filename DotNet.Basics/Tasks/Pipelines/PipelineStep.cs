using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks.Pipelines
{
    public abstract class PipelineStep<T> : ManagedTask<T> where T : class, new()
    {
        protected PipelineStep() : this(null)
        { }

        protected PipelineStep(string name) : base(name)
        { }

        protected override Task InnerRunAsync(T args, CancellationToken ct)
        {
            return RunImpAsync(args, ct);
        }

        protected abstract Task RunImpAsync(T args, CancellationToken ct);
    }
}
