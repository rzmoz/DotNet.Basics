using System.Threading;
using System.Threading.Tasks;
using DotNet.Standard.Sys;
using DotNet.Standard.Tasks;
using DotNet.Standard.Tasks.Pipelines;

namespace DotNet.Standard.Tests.Tasks.Pipelines.PipelineHelpers
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
