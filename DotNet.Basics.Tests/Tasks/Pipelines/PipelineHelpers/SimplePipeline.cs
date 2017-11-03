using System;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Pipelines;

namespace DotNet.Basics.Tests.Pipelines.PipelineHelpers
{
    public class SimplePipeline : Pipeline<EventArgs<int>>
    {
        public SimplePipeline(IServiceProvider serviceProvider)
        {
            AddBlock("MyBlock").AddStep<IncrementArgsStep>(serviceProvider);
        }
    }
}
