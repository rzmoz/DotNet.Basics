using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Standard.Tasks;
using DotNet.Standard.Tasks.Pipelines;

namespace DotNet.Standard.Tests.Tasks.Pipelines.PipelineHelpers
{
    public class SimpleStep : PipelineStep<EventArgs>
    {
        protected override Task RunImpAsync(EventArgs args, TaskIssueList issues, CancellationToken ct)
        {
            return Task.CompletedTask;
        }
    }
}
