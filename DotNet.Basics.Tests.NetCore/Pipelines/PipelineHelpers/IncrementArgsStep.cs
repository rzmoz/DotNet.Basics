using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;
using DotNet.Basics.Pipelines;

namespace DotNet.Basics.Tests.Pipelines.PipelineHelpers
{
    public class IncrementArgsStep : PipelineStep<EventArgs<int>>
    {
        protected override Task RunImpAsync(EventArgs<int> args, TaskIssueList issues, CancellationToken ct)
        {
            args.Value = IncrementByOne(args.Value);
            return Task.FromResult("");
        }
        public int IncrementByOne(int input)
        {
            return ++input;
        }
    }
}
