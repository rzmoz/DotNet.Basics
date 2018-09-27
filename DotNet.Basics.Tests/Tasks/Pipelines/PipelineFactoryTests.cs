using System.Linq;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Pipelines;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Tasks.Pipelines
{
    public class PipelineFactoryTests
    {
        [Fact]
        public void GetPipelineTypes_ScanForPipelines_PipelinesAreFound()
        {
            var pipelines = typeof(PipelineFactoryTests).Assembly.GetPipelineTypes();
            pipelines.Count().Should().Be(3);
        }

        [Fact]
        public void GetPipelineStepTypes_ScanForPipelineSteps_PipelineStepsAreFound()
        {
            var steps = typeof(PipelineFactoryTests).Assembly.GetPipelineStepTypes();
            steps.Count().Should().Be(6);
        }

        public class PipelineFromGeneric : Pipeline<EventArgs<int>>
        { }
    }
}
