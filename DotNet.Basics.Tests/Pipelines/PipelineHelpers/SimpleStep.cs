using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Pipelines;

namespace DotNet.Basics.Tests.Pipelines.PipelineHelpers
{
    public class SimpleStep : PipelineStep<EventArgs>
    {
        protected override Task RunImpAsync(EventArgs args, LogDispatcher log, CancellationToken ct)
        {
            return Task.CompletedTask;
        }
    }
}
