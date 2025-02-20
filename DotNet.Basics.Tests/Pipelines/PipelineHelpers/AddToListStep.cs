using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNet.Basics.Pipelines;

namespace DotNet.Basics.Tests.Pipelines.PipelineHelpers
{
    public class AddToListStep : PipelineStep<List<int>>
    {
        protected override async Task<int> RunImpAsync(List<int> args)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(int.Parse(Name) * 500));
            args.Add(int.Parse(Name));
            return 0;
        }
    }
}
