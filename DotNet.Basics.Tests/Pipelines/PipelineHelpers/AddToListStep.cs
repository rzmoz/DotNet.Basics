using System.Collections.Generic;
using System.Threading.Tasks;
using DotNet.Basics.Pipelines;

namespace DotNet.Basics.Tests.Pipelines.PipelineHelpers
{
    public class AddToListStep : PipelineStep<List<string>>
    {
        protected override Task<int> RunImpAsync(List<string> args)
        {
            args.Add(Name);
            return Task.FromResult(0);
        }
    }
}
