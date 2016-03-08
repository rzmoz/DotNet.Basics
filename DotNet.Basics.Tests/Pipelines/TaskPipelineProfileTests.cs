using System;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Ioc;
using DotNet.Basics.Pipelines;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Pipelines
{
    [TestFixture]
    public class TaskPipelineProfileTests
    {
        [Test]
        public async Task Ctor_InitProfiling_ProfilingsAreAccessible()
        {
            var pipeline = new TaskPipeline<EventArgs>();

            //add 3 step blocks with increasing number of steps in each

            pipeline.AddBlock(async (args, logger) => await Task.Delay(1.Milliseconds()));

            pipeline.AddBlock(async (args, logger) => await Task.Delay(1.Milliseconds()),
                              async (args, logger) => await Task.Delay(1.Milliseconds()));

            pipeline.AddBlock(async (args, logger) => await Task.Delay(1.Milliseconds()),
                              async (args, logger) => await Task.Delay(1.Milliseconds()),
                              async (args, logger) => await Task.Delay(1.Milliseconds()));

            var runner = new TaskPipelineRunner();

            var result = await runner.RunAsync(pipeline);

            result.Profile.BlockProfiles.Count.Should().Be(3);
            var blocks = result.Profile.BlockProfiles.OrderBy(block => block.StartTime);
            blocks.Skip(0).First().StepProfiles.Count().Should().Be(1);
            blocks.Skip(1).First().StepProfiles.Count().Should().Be(2);
            blocks.Skip(2).First().StepProfiles.Count().Should().Be(3);
        }


        [Test]
        public async Task Profile_ProfileSums_ProfilesAddsUp()
        {
            var pipeline = new TaskPipeline<EventArgs>();

            //add 3 step blocks with increasing number of steps in each

            pipeline.AddBlock(async (args, logger) => await Task.Delay(1000.Milliseconds()));

            pipeline.AddBlock(async (args, logger) => await Task.Delay(1000.Milliseconds()),
                              async (args, logger) => await Task.Delay(2000.Milliseconds()));

            var runner = new TaskPipelineRunner();

            var result = await runner.RunAsync(pipeline);


            var totalTime = result.Profile.Duration.Ticks;
            var summedBlockTime = result.Profile.BlockProfiles.Sum(bt => bt.Duration.Ticks);

            var firstBlock = result.Profile.BlockProfiles.Skip(0).First();
            var secondBlock = result.Profile.BlockProfiles.Skip(1).First();

            //ensures that the full profile takes at least as much time as all blocks since they're executed in sequence
            totalTime.Should().BeGreaterOrEqualTo(summedBlockTime);

            //first block
            var slowestFirstBlockStep = firstBlock.StepProfiles.OrderByDescending(st => st.Duration).First();
            slowestFirstBlockStep.Duration.Should().BeCloseTo(1000.Milliseconds(), 250);
            firstBlock.Duration.Ticks.Should().BeGreaterOrEqualTo(slowestFirstBlockStep.Duration.Ticks);

            //second block
            var slowestSecondBlockStep = secondBlock.StepProfiles.OrderByDescending(st => st.Duration).First();
            slowestSecondBlockStep.Duration.Should().BeCloseTo(2000.Milliseconds(), 250);
            secondBlock.Duration.Ticks.Should().BeGreaterOrEqualTo(slowestSecondBlockStep.Duration.Ticks);
        }

        [Test]
        public async Task Ctor_DefaultBlockAndStepNames_StepsAreNamed()
        {
            var pipeline = new TaskPipeline<EventArgs>();

            pipeline.AddBlock(async (args, logger) => await Task.Delay(1.Milliseconds()));
            pipeline.AddBlock(async (args, logger) => await Task.Delay(1.Milliseconds()));

            var runner = new TaskPipelineRunner();

            var result = await runner.RunAsync(pipeline);

            result.Profile.Name.Should().Be("TaskPipeline`1");
            result.Profile.BlockProfiles.Skip(0).First().Name.Should().Be("TaskBlock 0");
            result.Profile.BlockProfiles.Skip(1).First().Name.Should().Be("TaskBlock 1");

            result.Profile.Duration.Should().BeCloseTo(2.Milliseconds(), 500);
        }

        [Test]
        public async Task Ctor_CustomBlockAndStepNames_StepsAreNamed()
        {

            var container = new CsbContainer();
            container.BindType<ClassThatIncrementArgsDependOn>();
            var pipeline = new TaskPipeline<TEventArgs<int>>();

            pipeline.AddBlock("MyBlock1").AddStep<IncrementArgsStep>();

            var runner = new TaskPipelineRunner(container);

            var result = await runner.RunAsync(pipeline);

            result.Profile.Name.Should().Be("TaskPipeline`1");
            result.Profile.BlockProfiles.Single().Name.Should().Be("MyBlock1");
            result.Profile.BlockProfiles.Single().StepProfiles.Single().Name.Should().Be("MyIncrementArgsStep");
        }
    }
}
