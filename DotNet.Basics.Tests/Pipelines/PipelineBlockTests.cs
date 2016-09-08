using System.Linq;
using DotNet.Basics.Pipelines;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Pipelines
{
    [TestFixture]
    public class PipelineBlockTests
    {
        [Test]
        public void Add_AddGenericSteps_StepsAreAdded()
        {
            var stepBlock = new PipelineBlock<EventArgs<int>>(null, null)
                .AddStep<IncrementArgsStep>()
                .AddStep<IncrementArgsStep>()
                .AddStep<IncrementArgsStep>()
                .AddStep<IncrementArgsStep>()
                .AddStep<IncrementArgsStep>();

            stepBlock.Count().Should().Be(5);
        }
    }
}
