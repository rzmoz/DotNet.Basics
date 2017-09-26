﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DotNet.Basics.Ioc;
using DotNet.Basics.Rest;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;
using DotNet.Basics.Tasks.Pipelines;
using DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Tasks.Pipelines
{
    public class PipelineTests
    {
        [Fact]
        public void RegisterPipelineSteps_RegisterSteps_StepsAndCtorParamsAreRegisteredRecursive()
        {
            AssertRegisterPipelineSteps(p => { p.AddStep<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>(); });
        }
        [Fact]
        public void RegisterPipelineSteps_RegisterBlock_BlocksAreRegistered()
        {
            AssertRegisterPipelineSteps(p => { p.AddBlock("MyBlock"); });
        }
        [Fact]
        public void LazyLoad_RegisterStepsInBlock_StepsAndCtorParamsAreRegisteredRecursive()
        {
            AssertRegisterPipelineSteps(p => { p.AddBlock("MyBlock").AddStep<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>(); });
        }

        private void AssertRegisterPipelineSteps(Action<Pipeline> addAction)
        {
            var builder = new IocBuilder(resolveConcreteTypesNotAlreadyRegistered: false);

            //register abstract types specifically
            builder.RegisterType<RestClient>().As<IRestClient>();
            builder.RegisterType<ClassThatTakesAnAbstractClassAsCtorParam>();
            //abstract dependency registrations are not overridden
            builder.RegisterPipelineSteps(typeof(PipelineTests).Assembly);
            var pipeline = new Pipeline(() => builder.Container);
            addAction(pipeline);

            Action action = async () => await pipeline.RunAsync(CancellationToken.None).ConfigureAwait(false);

            action.ShouldNotThrow();
        }

        [Theory]
        [InlineData(Invoke.Parallel)]
        [InlineData(Invoke.Sequential)]
        public async Task RunAsync_AddIssuesFromDerivedStepClass_IssuesAreCollectedAndAggregatedInFinalResult(Invoke invoke)
        {
            var pipeline = new Pipeline(invoke);
            var count = 15;
            foreach (var i in Enumerable.Range(0, count))
                pipeline.AddStep<AddIssueStep>();

            var result = await pipeline.RunAsync(CancellationToken.None).ConfigureAwait(false);

            result.Issues.Count.Should().Be(count);
            result.Issues.All(i => i.Message == nameof(AddIssueStep)).Should().BeTrue();
            result.Issues.All(i => i.Exception == null).Should().BeTrue();
        }

        [Fact]
        public void Ctor_LazyLoadStepName_NameIsSetOnAdd()
        {
            var pipeline = new Pipeline();
            pipeline.AddStep<AddIssueStep>();

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
            pipeline.AddStep((args, issues, ct) => new AncestorStep().RunAsync(args, ct));
            pipeline.AddStep<DescendantStep>();

            //assert
            var argsUpdated = await pipeline.RunAsync(argsInit, CancellationToken.None).ConfigureAwait(false);
            argsInit.AncestorUpdated.Should().BeTrue();
            argsInit.DescendantUpdated.Should().BeTrue();
        }

        [Theory]
        [InlineData(Invoke.Parallel)]
        [InlineData(Invoke.Sequential)]
        public async Task RunAsync_TaskCancellation_PipelineIsCancelled(Invoke invoke)
        {
            var pipeline = new Pipeline(invoke);

            var ts = new CancellationTokenSource();
            ts.Cancel();
            ts.IsCancellationRequested.Should().BeTrue();
            var counter = 0;
            var stepCount = 101;

            for (var i = 0; i < stepCount; i++)
                pipeline.AddStep((args, issues, ct) => Task.FromResult(++counter));

            await pipeline.RunAsync(ts.Token).ConfigureAwait(false);

            pipeline.Tasks.Count().Should().Be(stepCount);
            counter.Should().Be(0);
        }

        [Fact]
        public async Task DisplayName_DisplayNameIsNotSet_TypeNameNameIsUsed()
        {
            var pipeline = new Pipeline<EventArgs<int>>();
            pipeline.AddStep<IncrementArgsStep>();
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
            var pipeline = new Pipeline();
            var block = pipeline.AddBlock("ParallelBlock", Invoke.Parallel);

            var runCount = 101;

            for (var i = 0; i < runCount; i++)
                block.AddStep(async (args, issues, ct) => await Task.Delay(TimeSpan.FromSeconds(1), ct));

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
            var pipeline = new Pipeline();
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
            var pipeline = new Pipeline<EventArgs<int>>();
            pipeline.AddStep<IncrementArgsStep>();
            pipeline.AddStep<IncrementArgsStep>();
            pipeline.AddBlock("1").AddStep<IncrementArgsStep>();
            pipeline.AddStep<IncrementArgsStep>();
            pipeline.AddBlock("2").AddStep<IncrementArgsStep>();

            var resultArgs = new EventArgs<int>();
            await pipeline.RunAsync(resultArgs ,CancellationToken.None).ConfigureAwait(false);
            resultArgs.Value.Should().Be(5);
        }

        [Fact]
        public void Add_AddGenericSteps_StepsAreAdded()
        {
            var block = new Pipeline();
            var taskCount = 7;

            foreach (var i in Enumerable.Range(0, taskCount))
                block.AddStep<ManagedTask<EventArgs>>();

            block.Tasks.Count().Should().Be(taskCount);
        }

        [Fact]
        public void Add_AddInlineTasks_TasksAreAdded()
        {
            var block = new Pipeline<EventArgs<int>>();
            var taskCount = 7;

            foreach (var i in Enumerable.Range(0, taskCount))
                block.AddStep((args, issues, ct) => { });

            block.Tasks.Count().Should().Be(taskCount);
        }

        [Theory]
        [InlineData(101, Invoke.Parallel)]
        [InlineData(10, Invoke.Sequential)]
        public async Task BlockInvokeStyle_Sequence_TasksAreRunAccordingToInvokeStyle(int stepCount, Invoke invoke)
        {
            var block = new Pipeline<EventArgs<int>>(invoke);
            int lockFlag = 0;

            var raceConditionEncountered = 0;
            for (var i = 0; i < stepCount; i++)
                block.AddStep(async (args, issues, ct) =>
                {
                    issues.Add(i.ToString());

                    if (Interlocked.CompareExchange(ref lockFlag, 1, 0) == 0)
                    {
                        Monitor.Enter(lockFlag);
                        args.Value = args.Value + 1;
                        await Task.Delay(TimeSpan.FromMilliseconds(100), CancellationToken.None).ConfigureAwait(false);
                        // free the lock.
                        Interlocked.Decrement(ref lockFlag);
                    }
                    else
                        raceConditionEncountered++;
                });

            var argsParam = new EventArgs<int>();

            var result = await block.RunAsync(argsParam, CancellationToken.None).ConfigureAwait(false);

            result.Issues.Count.Should().Be(stepCount);

            if (invoke == Invoke.Parallel)
            {
                argsParam.Value.Should().BeLessThan(stepCount);
                raceConditionEncountered.Should().BeGreaterThan(0);
            }
            else
            {
                argsParam.Value.Should().Be(stepCount);
                raceConditionEncountered.Should().Be(0);
            }
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

            var pipeline = new Pipeline<EventArgs<int>>();
            pipeline.AddBlock().AddStep<IncrementArgsStep>();

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