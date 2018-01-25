using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;
using DotNet.Basics.Tasks.Pipelines;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers
{
    public class AddIssueStep : PipelineStep<EventArgs>
    {
        protected override Task RunImpAsync(EventArgs args, TaskIssueList issues, CancellationToken ct)
        {
            var issue = nameof(AddIssueStep);
            issues.Add(LogLevel.Error, issue);
            return Task.FromResult("");
        }
    }
}
