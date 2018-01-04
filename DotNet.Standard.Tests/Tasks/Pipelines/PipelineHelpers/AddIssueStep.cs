using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Standard.Tasks;
using DotNet.Standard.Tasks.Pipelines;
using Microsoft.Extensions.Logging;

namespace DotNet.Standard.Tests.Tasks.Pipelines.PipelineHelpers
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
