using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Pipelines;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tests.Pipelines.PipelineHelpers
{
    public class IncrementArgsStep : PipelineStep<EventArgs<int>>
    {
        protected override Task<int> RunImpAsync(EventArgs<int> args)
        {
            args.Value = IncrementByOne(args.Value);
            return Task.FromResult(0);
        }

        public int IncrementByOne(int input)
        {
            return ++input;
        }
    }
}
