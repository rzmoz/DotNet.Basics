using System;
using System.Linq;
using DotNet.Basics.Pipelines;
using DotNet.Basics.Sys;
using DotNet.Basics.Tests.Pipelines.PipelineHelpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotNet.Basics.Tests.Pipelines
{
    public class PipelineExtensionsTests
    {
        [Fact]
        public void AddPipelines_AddToServiceCollection_PipelinesAreAdded()
        {
            var services = new ServiceCollection();

            //act
            services.AddPipelines([typeof(PipelineExtensionsTests).Assembly]);

            var provider = services.BuildServiceProvider();

            var pipelineFromPipeline = provider.GetService<PipelineFromPipeline>();
            pipelineFromPipeline.Should().BeOfType<PipelineFromPipeline>();

            var pipelineFromPipelinePipeline = provider.GetService<PipelineFromPipelinePipeline>();
            pipelineFromPipelinePipeline.Should().BeOfType<PipelineFromPipelinePipeline>();
            pipelineFromPipelinePipeline.Tasks.Count.Should().Be(1);

            var simpleStep = provider.GetService<SimpleStep>();
            simpleStep.Should().BeOfType<SimpleStep>();

        }

        public class PipelineFromPipeline(IServiceProvider services) : Pipeline<EventArgs>(services)
        { }

        public class PipelineFromPipelinePipeline : PipelineFromPipeline
        {
            public PipelineFromPipelinePipeline(IServiceProvider services) : base(services)
            {
                AddStep<SimpleStep>();
            }
        }
    }
}
