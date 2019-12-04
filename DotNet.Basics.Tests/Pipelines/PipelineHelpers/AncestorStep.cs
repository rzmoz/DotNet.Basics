using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Pipelines;

namespace DotNet.Basics.Tests.Pipelines.PipelineHelpers
{
    public class AncestorStep : PipelineStep<AncestorArgs>
    {
        protected override Task RunImpAsync(AncestorArgs args, ILogger log, CancellationToken ct)
        {
            args.AncestorUpdated = true;
            return Task.CompletedTask;
        }
    }
}
