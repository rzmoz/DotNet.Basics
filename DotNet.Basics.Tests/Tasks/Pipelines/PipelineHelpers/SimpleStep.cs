using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks.Pipelines;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers
{
    public class SimpleStep : PipelineStep<EventArgs>
    {
        protected override Task RunImpAsync(EventArgs args, TaskIssueList issues, CancellationToken ct)
        {
            return Task.CompletedTask;
        }
    }
}
