using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks.Pipelines;

namespace DotNet.Basics.Tests.Tasks.Pipelines
{
    public class DescendantStep : PipelineStep<DescendantArgs>
    {
        protected override Task InnerRunAsync(DescendantArgs args, CancellationToken ct)
        {
            args.DescendantUpdated = true;
            return base.InnerRunAsync(args, ct);
        }
    }
}
