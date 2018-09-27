using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Pipelines;

namespace DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers
{
    public class IncrementArgsStep : PipelineStep<EventArgs<int>>
    {
        protected override Task RunImpAsync(EventArgs<int> args, ConcurrentLog log, CancellationToken ct)
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
