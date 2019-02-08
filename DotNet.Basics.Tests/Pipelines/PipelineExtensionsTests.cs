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
            services.AddPipelines(typeof(PipelineExtensionsTests).Assembly);

            var provider = services.BuildServiceProvider();

            var step = provider.GetService<PipelineFromPipeline>();
            step.Should().BeOfType<PipelineFromPipeline>();
        }

        [Fact]
        public void AddPipelineTypes_AddToServiceCollection_StepsAreAdded()
        {
            var services = new ServiceCollection();

            //act
            services.AddPipelineSteps(typeof(PipelineExtensionsTests).Assembly);

            var provider = services.BuildServiceProvider();

            var step = provider.GetService<SimpleStep>();
            step.Should().BeOfType<SimpleStep>();
        }

        [Fact]
        public void GetPipelineTypes_ScanForPipelines_PipelinesAreFound()
        {
            var pipelines = typeof(PipelineExtensionsTests).Assembly.GetPipelineTypes();
            pipelines.Count().Should().Be(4);
        }

        [Fact]
        public void GetPipelineStepTypes_ScanForPipelineSteps_PipelineStepsAreFound()
        {
            var steps = typeof(PipelineExtensionsTests).Assembly.GetPipelineStepTypes();
            steps.Count().Should().Be(5);
        }

        public class PipelineFromPipeline : Pipeline<EventArgs<int>>
        { }
        public class PipelineFromPipelinePipeline : PipelineFromPipeline
        { }
    }
}
