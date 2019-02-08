using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Pipelines;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tests.Pipelines.PipelineHelpers
{
    public class IncrementArgsStep : PipelineStep<EventArgs<int>>
    {
        protected override Task RunImpAsync(EventArgs<int> args, CancellationToken ct)
        {
            args.Value = IncrementByOne(args.Value);
            return Task.CompletedTask;
        }

        public int IncrementByOne(int input)
        {
            return ++input;
        }
    }
}
