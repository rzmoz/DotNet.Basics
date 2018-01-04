using System.Linq;
using DotNet.Standard.Sys;
using DotNet.Standard.Tasks.Pipelines;
using FluentAssertions;
using Xunit;

namespace DotNet.Standard.Tests.Tasks.Pipelines
{
    public class PipelineFactoryTests
    {
        [Fact]
        public void GetPipelineTypes_ScanForPipelines_PipelinesAreFound()
        {
            var steps = typeof(PipelineFactoryTests).Assembly.GetPipelineTypes();
            steps.Count().Should().Be(2);
        }

        [Fact]
        public void GetPipelineStepTypes_ScanForPipelineSteps_PipelineStepsAreFound()
        {
            var pipelines = typeof(PipelineFactoryTests).Assembly.GetPipelineStepTypes();
            pipelines.Count().Should().Be(6);
        }

        public class PipelineFromGeneric : Pipeline<EventArgs<int>>
        { }

        public class PipelineFromNonGeneric : Pipeline
        { }
    }
}
