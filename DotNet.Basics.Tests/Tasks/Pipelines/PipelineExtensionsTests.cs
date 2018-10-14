using System.Linq;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Pipelines;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Tasks.Pipelines
{
    public class PipelineExtensionsTests
    {
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
            steps.Count().Should().Be(6);
        }

        public class PipelineFromPipeline : Pipeline<EventArgs<int>>
        { }
        public class PipelineFromPipelinePipeline : PipelineFromPipeline
        { }
    }
}
