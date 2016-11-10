using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DotNet.Basics.Ioc;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;
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

        [Theory]
        [InlineData(Invoke.Parallel)]
        [InlineData(Invoke.Sequential)]
        public async Task RunAsync_AddIssuesFromDerivedStepClass_IssuesAreCollectedAndAggregatedInFinalResult(Invoke invoke)
        {
            var pipeline = new Pipeline(invoke);
            var count = 15;
            foreach (var i in Enumerable.Range(0, count))
                pipeline.AddTask<AddIssueStep>();

            var result = await pipeline.RunAsync(CancellationToken.None).ConfigureAwait(false);

            result.Issues.Count.Should().Be(count);
            result.Issues.All(i => i.Message == nameof(AddIssueStep)).Should().BeTrue();
            result.Issues.All(i => i.Exception == null).Should().BeTrue();
        }

        [Fact]
        public void Ctor_LazyLoadStepName_NameIsSetOnAdd()
        {
            var pipeline = new Pipeline();
            pipeline.AddTask<AddIssueStep>();

            pipeline.Tasks.Single().Name.Should().Be(nameof(AddIssueStep));
        }
        [Fact]
        public async Task Ctor_ArgsInheritanceHierarchy_StepsWithAcenstorArgsCanBeUsedInPipeline()
        {
            var argsInit = new DescendantArgs();
            argsInit.AncestorUpdated.Should().BeFalse();
            argsInit.DescendantUpdated.Should().BeFalse();
            var pipeline = new Pipeline<DescendantArgs>();

            //act
            pipeline.AddTask((args, issues, ct) => new AncestorStep().RunAsync(args, ct));
            pipeline.AddTask<DescendantStep>();

            //assert
            var argsUpdated = await pipeline.RunAsync(argsInit, CancellationToken.None).ConfigureAwait(false);
            argsUpdated.Args.AncestorUpdated.Should().BeTrue();
            argsUpdated.Args.DescendantUpdated.Should().BeTrue();
        }

        [Theory]
        [InlineData(Invoke.Parallel)]
        [InlineData(Invoke.Sequential)]
        public async Task RunAsync_TaskCancellation_PipelineIsCancelled(Invoke invoke)
        {
            var pipeline = new Pipeline(_builder.Container, invoke);

            var ts = new CancellationTokenSource();
            ts.Cancel();
            ts.IsCancellationRequested.Should().BeTrue();
            var counter = 0;
            var stepCount = 101;

            for (var i = 0; i < stepCount; i++)
                pipeline.AddTask((args, issues, ct) => Task.FromResult(++counter));

            await pipeline.RunAsync(ts.Token).ConfigureAwait(false);

            pipeline.Tasks.Count().Should().Be(stepCount);
            counter.Should().Be(0);
        }

        [Fact]
        public async Task DisplayName_DisplayNameIsNotSet_TypeNameNameIsUsed()
        {
            var pipeline = new Pipeline<EventArgs<int>>(_builder.Container);
            pipeline.AddTask<IncrementArgsStep>();
            string stepName = null;

            pipeline.Started += (e) =>
            {
                if (e.Name.EndsWith("step", StringComparison.OrdinalIgnoreCase))
                    stepName = e.Name;
            };

            await pipeline.RunAsync().ConfigureAwait(false);

            stepName.Should().Be(nameof(IncrementArgsStep));
        }

        [Fact]
        public async Task RunAsync_AllInParallel_AllStepsAreRunInParallel()
        {
            var pipeline = new Pipeline<EventArgs<int>>(_builder.Container);
            var block = pipeline.AddBlock("ParallelBlock", Invoke.Parallel);

            var runCount = 101;

            for (var i = 0; i < runCount; i++)
                block.AddTask(async (args, issues, ct) => await Task.Delay(TimeSpan.FromSeconds(1), ct));

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

            pipeline.AddBlock("1", async (args, issues, ct) => { task1Called = true; await Task.Delay(TimeSpan.FromMilliseconds(200), ct); });
            pipeline.AddBlock("2", (args, issues, ct) =>
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
            pipeline.AddTask<IncrementArgsStep>();
            pipeline.AddTask<IncrementArgsStep>();
            pipeline.AddBlock("1").AddTask<IncrementArgsStep>();
            pipeline.AddTask<IncrementArgsStep>();
            pipeline.AddBlock("2").AddTask<IncrementArgsStep>();

            var args = await pipeline.RunAsync(CancellationToken.None).ConfigureAwait(false);
            args.Args.Value.Should().Be(5);
        }

        [Fact]
        public async Task RunAsync_Events_StartAndEndEventsAreRaised()
        {
            //arrange
            string pipelineStarted = string.Empty;
            string pipelineEnded = string.Empty;
            string blockStarted = string.Empty;
            string blockEnded = string.Empty;
            string stepStarted = string.Empty;
            string stepEnded = string.Empty;

            var pipeline = new Pipeline<EventArgs<int>>(_builder.Container);
            pipeline.AddBlock().AddTask<IncrementArgsStep>();


            pipeline.Started += args =>
            {
                if (args.Name.EndsWith("step", StringComparison.OrdinalIgnoreCase))
                    stepStarted = args.Name;
                else if (args.Name.StartsWith("block", StringComparison.OrdinalIgnoreCase))
                    blockStarted = args.Name;
                else if (args.Name.StartsWith("pipeline", StringComparison.OrdinalIgnoreCase))
                    pipelineStarted = args.Name;
            };
            pipeline.Ended += args =>
            {
                if (args.Name.EndsWith("step", StringComparison.OrdinalIgnoreCase))
                    stepEnded = args.Name;
                else if (args.Name.StartsWith("block", StringComparison.OrdinalIgnoreCase))
                    blockEnded = args.Name;
                else if (args.Name.StartsWith("pipeline", StringComparison.OrdinalIgnoreCase))
                    pipelineEnded = args.Name;
            };

            //act
            await pipeline.RunAsync().ConfigureAwait(false);

            //assert
            pipelineStarted.Should().Be("Pipeline`1", nameof(pipelineStarted));
            pipelineEnded.Should().Be("Pipeline`1", nameof(pipelineEnded));
            blockStarted.Should().Be("Block 0", nameof(blockStarted));
            blockEnded.Should().Be("Block 0", nameof(blockEnded));
            stepStarted.Should().Be("IncrementArgsStep", nameof(stepStarted));
            stepEnded.Should().Be("IncrementArgsStep", nameof(stepEnded));
        }
    }
}
