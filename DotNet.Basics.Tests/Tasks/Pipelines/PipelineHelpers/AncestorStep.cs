using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;
using DotNet.Basics.Tasks.Pipelines;

namespace DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers
{
    public class AncestorStep : PipelineStep<AncestorArgs>
    {
        protected override Task RunImpAsync(AncestorArgs args, TaskIssueList issues, CancellationToken ct)
        {
            args.AncestorUpdated = true;
            return Task.FromResult("");
        }
    }
}
