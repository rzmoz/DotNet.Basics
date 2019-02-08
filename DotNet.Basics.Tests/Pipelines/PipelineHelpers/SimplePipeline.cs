using DotNet.Basics.Pipelines;
using DotNet.Basics.Sys;

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
