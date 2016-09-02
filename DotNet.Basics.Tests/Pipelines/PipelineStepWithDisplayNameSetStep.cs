using System;
using System.Threading.Tasks;
using DotNet.Basics.Sys;
using DotNet.Basics.Pipelines;

namespace DotNet.Basics.Tests.Pipelines
{
    public class PipelineStepWithDisplayNameSetStep : PipelineStep<EventArgs>
    {
        public PipelineStepWithDisplayNameSetStep()
        {
            DisplayName = "MyDisplayName";
        }

        public override async Task RunAsync(EventArgs args)
        {
            await Task.Delay(1.MilliSeconds()).ConfigureAwait(false);//silence compiler warning
            Console.WriteLine($"Display name set to {DisplayName}");
        }
    }
}
