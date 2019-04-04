using System;
using System.Threading.Tasks;
using DotNet.Basics.Pipelines;
using DotNet.Basics.Tests.Pipelines.PipelineHelpers;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Pipelines
{
    public class PipelineStepTests
    {
        [Fact]
        public async Task MessagedLogged_LogMessage_MessageHasContext()
        {
            var taskReceived = string.Empty;

            var task = new DescendantStep();

            task.MessageLogged += (lvl, msg, e) => taskReceived = msg;

            //act
            await task.RunAsync(new DescendantArgs()).ConfigureAwait(false);

            //assert
            taskReceived.Should().Be($"Descendant / Hello World!");
        }

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
