using System.Threading;
using System.Threading.Tasks;
using DotNet.Standard.Tasks;
using DotNet.Standard.Tasks.Pipelines;

namespace DotNet.Standard.Tests.Tasks.Pipelines.PipelineHelpers
{
    public class DescendantStep : PipelineStep<DescendantArgs>
    {
        protected override Task RunImpAsync(DescendantArgs args, TaskIssueList issues, CancellationToken ct)
        {
            args.DescendantUpdated = true;
            return Task.FromResult("");
        }
    }
}
