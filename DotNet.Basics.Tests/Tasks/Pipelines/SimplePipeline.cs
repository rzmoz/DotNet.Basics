using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Pipelines;

namespace DotNet.Basics.Tests.Tasks.Pipelines
{
    public class SimplePipeline : Pipeline<EventArgs<int>>
    {
        public SimplePipeline()
        {
            AddBlock().AddStep<IncrementArgsStep>();
        }
    }
}
