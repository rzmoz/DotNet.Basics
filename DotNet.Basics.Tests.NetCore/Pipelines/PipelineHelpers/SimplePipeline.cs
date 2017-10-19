using DotNet.Basics.Sys;
using DotNet.Basics.Pipelines;

namespace DotNet.Basics.Tests.Pipelines.PipelineHelpers
{
    public class SimplePipeline : Pipeline<EventArgs<int>>
    {
        public SimplePipeline()
        {
            AddBlock("MyBlock").AddStep<IncrementArgsStep>();
        }
    }
}
