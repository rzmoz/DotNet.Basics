using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Tasks.Pipelines;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers
{
    public class AddLogEntryStep : PipelineStep<EventArgs>
    {
        protected override Task RunImpAsync(EventArgs args, ConcurrentLog log, CancellationToken ct)
        {
            var entry = nameof(AddLogEntryStep);
            log.Add(LogLevel.Error, entry);
            return Task.FromResult(string.Empty);
        }
    }
}
