using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Pipelines;

namespace DotNet.Basics.Tests.Pipelines.PipelineHelpers
{
    public class AncestorStep : PipelineStep<AncestorArgs>
    {
        protected override Task<int> RunImpAsync(AncestorArgs args)
        {
            args.AncestorUpdated = true;
            return Task.FromResult(0);
        }
    }
}
