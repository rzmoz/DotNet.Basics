using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Pipelines;

namespace DotNet.Basics.Tests.Pipelines.PipelineHelpers
{
    public class SimpleStep : PipelineStep<EventArgs>
    {
        protected override Task<int> RunImpAsync(EventArgs args, CancellationToken ct)
        {
            return Task.FromResult(0);
        }
    }
}
