using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Pipelines;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tests.Pipelines
{
    public class IncrementArgsStep : PipelineStep<EventArgs<int>>
    {
        private readonly ClassThatIncrementArgsDependOn _classThatIncrementArgsDependOn;

        public IncrementArgsStep(ClassThatIncrementArgsDependOn classThatIncrementArgsDependOn) : base(null)
        {
            _classThatIncrementArgsDependOn = classThatIncrementArgsDependOn;
        }

        protected override async Task InnerRunAsync(EventArgs<int> args, CancellationToken ct)
        {
            await Task.Delay(1.MilliSeconds(), ct).ConfigureAwait(false);//silence compiler warning
            args.Value = _classThatIncrementArgsDependOn.IncrementByOne(args.Value);
            Console.WriteLine($"Value is now: {args.Value}");
        }
    }
}
