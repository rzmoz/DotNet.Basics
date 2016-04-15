using System.Linq;
using DotNet.Basics.Ioc;
using DotNet.Basics.Pipelines;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Pipelines
{
    [TestFixture]
    public class StepBlockTests
    {
        
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void Add_AddGenericSteps_StepsAreAdded()
        {
            var stepBlock = new PipelineBlock<TEventArgs<int>>()
                .AddStep<IncrementArgsStep>()
                .AddStep<IncrementArgsStep>()
                .AddStep<IncrementArgsStep>()
                .AddStep<IncrementArgsStep>()
                .AddStep<IncrementArgsStep>();

            stepBlock.Count().Should().Be(5);
        }
    }
}
