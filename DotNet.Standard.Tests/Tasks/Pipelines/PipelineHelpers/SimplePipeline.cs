using System;
using DotNet.Standard.Sys;
using DotNet.Standard.Tasks.Pipelines;

namespace DotNet.Standard.Tests.Tasks.Pipelines.PipelineHelpers
{
    public class SimplePipeline : Pipeline<EventArgs<int>>
    {
        public SimplePipeline(IServiceProvider serviceProvider)
        {
            AddBlock("MyBlock").AddStep<IncrementArgsStep>(serviceProvider);
        }
    }
}
