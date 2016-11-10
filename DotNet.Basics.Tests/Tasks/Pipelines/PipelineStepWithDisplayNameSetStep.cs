using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;
using DotNet.Basics.Tasks.Pipelines;

namespace DotNet.Basics.Tests.Tasks.Pipelines
{
    public class PipelineStepWithDisplayNameSetStep : PipelineStep<EventArgs>
    {
        public PipelineStepWithDisplayNameSetStep() : base("ThisStepHasCustomName")
        {
        }
        
        protected override Task RunImpAsync(EventArgs args, TaskIssueList issues, CancellationToken ct)
        {
            DebugOut.WriteLine($@"Display name set to {Name}");
            return Task.FromResult("");
        }
    }
}
