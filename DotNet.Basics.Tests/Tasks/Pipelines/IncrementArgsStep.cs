using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;
using DotNet.Basics.Tasks.Pipelines;

namespace DotNet.Basics.Tests.Tasks.Pipelines
{
    public class IncrementArgsStep : PipelineStep<EventArgs<int>>
    {
        private readonly ClassThatIncrementArgsDependOn _classThatIncrementArgsDependOn;

        public IncrementArgsStep(ClassThatIncrementArgsDependOn classThatIncrementArgsDependOn) : base(null)
        {
            _classThatIncrementArgsDependOn = classThatIncrementArgsDependOn;
        }

        protected override Task RunImpAsync(EventArgs<int> args, TaskIssueList issues, CancellationToken ct)
        {
            args.Value = _classThatIncrementArgsDependOn.IncrementByOne(args.Value);
            DebugOut.WriteLine($@"Value is now: {args.Value}");
            return Task.FromResult("");
        }
    }
}
