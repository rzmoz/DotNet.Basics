using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;
using DotNet.Basics.Tasks.Pipelines;

namespace DotNet.Basics.Tests.Tasks.Pipelines
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
