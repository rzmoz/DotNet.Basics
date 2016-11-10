using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DotNet.Basics.Ioc;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Pipelines;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Tasks.Pipelines
{
    
    public class PipelineTests
    {
        private readonly IocBuilder _builder;

        
        public PipelineTests()
        {
            _builder = new IocBuilder();
            _builder.RegisterType<ClassThatIncrementArgsDependOn>().AsSelf();
        }

        [Fact]
        public async Task Ctor_ArgsInheritanceHierarchy_StepsWithAcenstorArgsCanBeUsedInPipeline()
        {
            var argsInit = new DescendantArgs();
            argsInit.AncestorUpdated.Should().BeFalse();
            argsInit.DescendantUpdated.Should().BeFalse();
            var pipeline = new Pipeline<DescendantArgs>();

            //act
            pipeline.AddStep((args, ct) => new AncestorStep().RunAsync(args, ct));
            pipeline.AddStep<DescendantStep>();

            //assert
            var argsUpdated = await pipeline.RunAsync(argsInit, CancellationToken.None).ConfigureAwait(false);
            argsUpdated.AncestorUpdated.Should().BeTrue();
            argsUpdated.DescendantUpdated.Should().BeTrue();
        }


        [Fact]
        public async Task RunAsync_TaskCancellation_PipelineIsCancelled()
        {
            var pipeline = new Pipeline(_builder.Container);

            var ts = new CancellationTokenSource();
            ts.Cancel();
            ts.IsCancellationRequested.Should().BeTrue();
            var counter = 0;
            var stepCount = 101;

            for (var i = 0; i < stepCount; i++)
                pipeline.AddStep((args, ct) => Task.FromResult(++counter));

            await pipeline.RunAsync(ts.Token).ConfigureAwait(false);

            pipeline.Count().Should().Be(stepCount);
            counter.Should().Be(1);
        }


        [Fact]
        public async Task DisplayName_DisplayNameIsSet_DisplayNameIsUsed()
        {
            var pipeline = new Pipeline(_builder.Container);
            pipeline.AddBlock("").AddStep<PipelineStepWithDisplayNameSetStep>();

            string stepName = null;

            pipeline.Started += (e) =>
            {
                if (e.TaskType == PipelineTaskTypes.Step)
                    stepName = e.Name;
            };

            await pipeline.RunAsync().ConfigureAwait(false);

            stepName.Should().Be("ThisStepHasCustomName");
        }

        [Fact]
        public async Task DisplayName_DisplayNameIsNotSet_TypeNameNameIsUsed()
        {
            var pipeline = new Pipeline<EventArgs<int>>(_builder.Container);
            pipeline.AddStep<IncrementArgsStep>();
            string stepName = null;

            pipeline.Started += (e) =>
            {
                if (e.TaskType == PipelineTaskTypes.Step)
                    stepName = e.Name;
            };

            await pipeline.RunAsync().ConfigureAwait(false);

            stepName.Should().Be(nameof(IncrementArgsStep));
        }

        [Fact]
        public async Task RunAsync_AllInParallel_AllStepsAreRunInParallel()
        {
            var pipeline = new Pipeline<EventArgs<int>>(_builder.Container);
            var block = pipeline.AddBlock("ParallelBlock", BlockRunType.Parallel);

            var runCount = 101;

            for (var i = 0; i < runCount; i++)
                block.AddStep(async (args, ct) => await Task.Delay(TimeSpan.FromSeconds(1), ct));

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await pipeline.RunAsync(CancellationToken.None).ConfigureAwait(false);
            stopwatch.Stop();

            //total timespan should be close to 1 second since tasks were run in parallel
            stopwatch.Elapsed.Should().BeCloseTo(TimeSpan.FromSeconds(6), 5000);
        }

        [Fact]
        public async Task RunAsync_BlockWait_StepsAreRunInBlockOrder()
        {
            var pipeline = new Pipeline<EventArgs<int>>(_builder.Container);
            var task1Called = false;
            var task2Called = false;

            pipeline.AddBlock("1", async (args, ct) => { task1Called = true; await Task.Delay(TimeSpan.FromMilliseconds(200), ct); });
            pipeline.AddBlock("2", (args, xct) =>
            {
                if (task1Called == false)
                    throw new ArgumentException("Task 1 not called");
                if (task2Called)
                    throw new ArgumentException("Task 2 called already");
                task2Called = true;
                return Task.FromResult("");
            });

            task1Called.Should().BeFalse();
            task2Called.Should().BeFalse();

            await pipeline.RunAsync(CancellationToken.None).ConfigureAwait(false);

            task1Called.Should().BeTrue();
            task2Called.Should().BeTrue();
        }


        [Fact]
        public async Task RunAsync_PassArgs_ArgsArePassedInPipeline()
        {
            var pipeline = new Pipeline<EventArgs<int>>(_builder.Container);
            pipeline.AddStep<IncrementArgsStep>();
            pipeline.AddStep<IncrementArgsStep>();
            pipeline.AddBlock("1").AddStep<IncrementArgsStep>();
            pipeline.AddStep<IncrementArgsStep>();
            pipeline.AddBlock("2").AddStep<IncrementArgsStep>();

            var args = await pipeline.RunAsync(CancellationToken.None).ConfigureAwait(false);
            args.Value.Should().Be(5);
        }

        [Fact]
        public async Task RunAsync_Events_StartAndEndEventsAreRaised()
        {
            //arrange
            var pipeline = new Pipeline<EventArgs<int>>(_builder.Container);
            pipeline.AddBlock(null).AddStep<IncrementArgsStep>();
            string pipelineStarted = string.Empty;
            string pipelineEnded = string.Empty;
            string blockStarted = string.Empty;
            string blockEnded = string.Empty;
            string stepStarted = string.Empty;
            string stepEnded = string.Empty;

            pipeline.Started += args =>
            {
                switch (args.TaskType)
                {
                    case PipelineTaskTypes.Step:
                        stepStarted = args.Name;
                        break;
                    case PipelineTaskTypes.Block:
                        blockStarted = args.Name;
                        break;
                    case PipelineTaskTypes.Pipeline:
                        pipelineStarted = args.Name;
                        break;
                }
            };
            pipeline.Ended += args =>
            {
                switch (args.TaskType)
                {
                    case PipelineTaskTypes.Step:
                        stepEnded = args.Name;
                        break;
                    case PipelineTaskTypes.Block:
                        blockEnded = args.Name;
                        break;
                    case PipelineTaskTypes.Pipeline:
                        pipelineEnded = args.Name;
                        break;
                }
            };

            //act
            await pipeline.RunAsync().ConfigureAwait(false);

            //assert
            pipelineStarted.Should().Be("Pipeline", nameof(pipelineStarted));
            pipelineEnded.Should().Be("Pipeline", nameof(pipelineEnded));
            blockStarted.Should().Be("Block 0", nameof(blockStarted));
            blockEnded.Should().Be("Block 0", nameof(blockEnded));
            stepStarted.Should().Be("IncrementArgsStep", nameof(stepStarted));
            stepEnded.Should().Be("IncrementArgsStep", nameof(stepEnded));
        }
    }
}
