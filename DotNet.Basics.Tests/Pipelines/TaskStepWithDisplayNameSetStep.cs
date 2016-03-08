using System;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;
using DotNet.Basics.Pipelines;

namespace DotNet.Basics.Tests.Pipelines
{
    public class TaskStepWithDisplayNameSetStep : TaskStep<EventArgs>
    {
        public TaskStepWithDisplayNameSetStep()
        {
            DisplayName = "MyDisplayName";
        }

        public override async Task RunAsync(EventArgs args, IDiagnostics logger)
        {
            await Task.Delay(1.MilliSeconds()).ConfigureAwait(false);//silence compiler warning
            logger.Log($"Display name set to {DisplayName}");
        }
    }
}
