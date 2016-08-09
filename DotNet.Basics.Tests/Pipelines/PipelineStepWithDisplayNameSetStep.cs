using System;
using System.Threading.Tasks;
using DotNet.Basics.Sys;
using DotNet.Basics.Pipelines;
using NLog;

namespace DotNet.Basics.Tests.Pipelines
{
    public class PipelineStepWithDisplayNameSetStep : PipelineStep<EventArgs>
    {
        public PipelineStepWithDisplayNameSetStep()
        {
            DisplayName = "MyDisplayName";
        }

        public override async Task RunAsync(EventArgs args, IPipelineLogger logger)
        {
            await Task.Delay(1.MilliSeconds()).ConfigureAwait(false);//silence compiler warning
            logger.Info($"Display name set to {DisplayName}");
        }
    }
}
