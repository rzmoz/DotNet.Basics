using System.Threading;
using System.Threading.Tasks;
using DotNet.Standard.Tasks;
using DotNet.Standard.Tasks.Pipelines;

namespace DotNet.Standard.Tests.Tasks.Pipelines.PipelineHelpers
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
