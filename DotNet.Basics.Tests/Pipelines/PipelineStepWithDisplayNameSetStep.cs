using System;
using System.Threading.Tasks;
using DotNet.Basics.Sys;
using DotNet.Basics.Pipelines;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Tests.Pipelines
{
    public class PipelineStepWithDisplayNameSetStep : PipelineStep<EventArgs>
    {
        public PipelineStepWithDisplayNameSetStep()
        {
            DisplayName = "MyDisplayName";
        }

        public override async Task RunAsync(EventArgs args, ILogger logger)
        {
            await Task.Delay(1.MilliSeconds()).ConfigureAwait(false);//silence compiler warning
            logger.LogInformation($"Display name set to {DisplayName}");
        }
    }
}
