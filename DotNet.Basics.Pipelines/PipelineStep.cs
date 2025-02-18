using System.Threading.Tasks;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.Pipelines
{
    public abstract class PipelineStep<T>(string name) : ManagedTask<T>(name, "Step")
    {
        protected PipelineStep() : this(null)
        { }

        protected override Task<int> InnerRunAsync(T args)
        {
            return RunImpAsync(args);
        }

        protected abstract Task<int> RunImpAsync(T args);
    }
}
