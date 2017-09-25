using System;
using Autofac;
using DotNet.Basics.Tasks.Pipelines;

namespace DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers
{
    public class PipelineWithContainer : Pipeline
    {
        public PipelineWithContainer(Func<IContainer> getContainer) : base(getContainer)
        {
            AddStep<AddIssueStep>();
        }

    }
}
