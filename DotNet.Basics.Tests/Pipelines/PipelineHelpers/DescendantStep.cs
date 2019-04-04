using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Pipelines;

namespace DotNet.Basics.Tests.Pipelines.PipelineHelpers
{
    public class DescendantStep : PipelineStep<DescendantArgs>
    {
        protected override Task RunImpAsync(DescendantArgs args, LogDispatcher log, CancellationToken ct)
        {
            args.DescendantUpdated = true;
            log.Debug("Hello World!");
            return Task.CompletedTask;
        }
    }
}
