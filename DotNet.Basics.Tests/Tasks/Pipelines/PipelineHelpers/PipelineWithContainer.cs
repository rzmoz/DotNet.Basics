using System;
using DotNet.Basics.Tasks.Pipelines;

namespace DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers
{
    public class PipelineWithContainer : Pipeline<EventArgs>
    {
        public PipelineWithContainer(IServiceProvider serviceProvider) : base()
        {
            AddStep<AddLogEntryStep>(serviceProvider);
        }
    }
}
