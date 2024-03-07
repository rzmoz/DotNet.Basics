using DotNet.Basics.Tests.Pipelines.PipelineHelpers;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Pipelines
{
    public class PipelineStepTests
    {
        [Fact]
        public void Name_IgnoreSuffix_SuffixIsRemovedFromName()
        {
            var step = new IncrementArgsStep();
            //assert
            step.Name.Should().Be("IncrementArgs");
            (step.Name + "Step").Should().Be(nameof(IncrementArgsStep));
        }
    }
}
