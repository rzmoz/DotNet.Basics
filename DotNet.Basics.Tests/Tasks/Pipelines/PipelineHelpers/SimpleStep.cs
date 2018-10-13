using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks.Pipelines;

namespace DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers
{
    public class SimpleStep : PipelineStep<EventArgs>
    {
        protected override Task RunImpAsync(EventArgs args, CancellationToken ct)
        {
            return Task.CompletedTask;
        }
    }
}
