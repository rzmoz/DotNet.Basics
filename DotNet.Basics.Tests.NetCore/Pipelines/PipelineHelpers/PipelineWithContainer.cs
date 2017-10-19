using System;
using Autofac;
using DotNet.Basics.Pipelines;

namespace DotNet.Basics.Tests.Pipelines.PipelineHelpers
{
    public class PipelineWithContainer : Pipeline
    {
        public PipelineWithContainer(Func<IContainer> getContainer) : base(getContainer)
        {
            AddStep<AddIssueStep>();
        }

    }
}
