using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;
using DotNet.Basics.Tasks.Pipelines;

namespace DotNet.Basics.Tests.Tasks.Pipelines
{
    public class AddIssueStep : PipelineStep<EventArgs>
    {
        protected override Task RunImpAsync(EventArgs args, TaskIssueList issues, CancellationToken ct)
        {
            var issue = nameof(AddIssueStep);
            DebugOut.WriteLine($"Adding issue: {issue}");
            issues.Add(issue);
            return Task.FromResult("");
        }
    }
}
