using System;
using DotNet.Standard.Tasks.Pipelines;

namespace DotNet.Standard.Tests.Tasks.Pipelines.PipelineHelpers
{
    public class PipelineWithContainer : Pipeline
    {
        public PipelineWithContainer(IServiceProvider serviceProvider) : base()
        {
            AddStep<AddIssueStep>(serviceProvider);
        }
    }
}
