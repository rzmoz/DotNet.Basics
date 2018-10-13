using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks.Pipelines;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers
{
    public class AddLogEntryStep : PipelineStep<EventArgs>
    {
        protected override Task RunImpAsync(EventArgs args, CancellationToken ct)
        {
            Log(LogLevel.Warning, nameof(AddLogEntryStep));
            return Task.CompletedTask;
        }
    }
}
