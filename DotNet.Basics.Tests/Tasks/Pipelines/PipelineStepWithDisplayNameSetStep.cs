using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Pipelines;

namespace DotNet.Basics.Tests.Tasks.Pipelines
{
    public class PipelineStepWithDisplayNameSetStep : PipelineStep<EventArgs>
    {
        public PipelineStepWithDisplayNameSetStep() : base("ThisStepHasCustomName")
        {
        }

        protected override async Task InnerRunAsync(EventArgs args, CancellationToken ct)
        {
            await Task.Delay(1.MilliSeconds(), ct).ConfigureAwait(false);//silence compiler warning
            Console.WriteLine($"Display name set to {Name}");
        }
    }
}
