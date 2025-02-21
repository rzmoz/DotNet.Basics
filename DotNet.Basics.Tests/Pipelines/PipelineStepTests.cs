using DotNet.Basics.Sys;
using DotNet.Basics.Tests.Pipelines.PipelineHelpers;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Pipelines
{
    public class PipelineStepTests
    {
        [Fact]
        public void Name_TitleCase_NameIsTitleCased()
        {
            var step = new IncrementArgsStep();
            //assert
            step.Name.Should().Be(nameof(IncrementArgsStep).ToTitleCase());
        }
    }
}
