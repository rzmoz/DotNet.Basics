using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Pipelines;

namespace DotNet.Basics.Tests.Pipelines.PipelineHelpers
{
    public class AddToListStep : PipelineStep<List<int>>
    {
        protected override async Task<int> RunImpAsync(List<int> args, CancellationToken cancellationToken = default)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(int.Parse(Name) * 500), cancellationToken);
            args.Add(int.Parse(Name));
            return 0;
        }
    }
}
