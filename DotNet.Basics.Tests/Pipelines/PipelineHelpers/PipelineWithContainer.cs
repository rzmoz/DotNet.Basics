using System;
using DotNet.Basics.Pipelines;

namespace DotNet.Basics.Tests.Pipelines.PipelineHelpers
{
    public class PipelineWithContainer : Pipeline<EventArgs>
    {
        public PipelineWithContainer(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            AddStep<SimpleStep>();
        }
    }
}
