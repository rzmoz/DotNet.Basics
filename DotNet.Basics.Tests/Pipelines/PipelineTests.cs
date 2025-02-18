using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;
using DotNet.Basics.Collections;
using DotNet.Basics.Pipelines;
using DotNet.Basics.Tests.Pipelines.PipelineHelpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotNet.Basics.Tests.Pipelines
{
    public class PipelineTests
    {
        [Fact]
        public void Name_IgnoreSuffix_SuffixIsRemovedFromName()
        {
            var pipelineName = "MyPipeline";
            var pipeline = new Pipeline<EventArgs>(pipelineName);
            //assert
            pipeline.Name.Should().Be("My");
            (pipeline.Name + "Pipeline").Should().Be(pipelineName);
        }
        [Fact]
        public void Name_IgnoreBlock_SuffixIsRemovedFromName()
        {
            var blockName = "MyBlock";
            var pipeline = new Pipeline<EventArgs>();
            var block = pipeline.AddBlock(blockName);
            //assert
            block.Name.Should().Be("My");
            (block.Name + "Block").Should().Be(blockName);
        }

        [Fact]
        public async Task Started_Events_EventsIsOnlyTriggeredOnce()
        {
            var pipelineName = "MyPipeline";
            var pipelineStarted = 0;

            var pipeline = new Pipeline<EventArgs>(pipelineName);
            var block = pipeline.AddBlock();

            block.AddStep((args, ct) => { });

            pipeline.Started += (e) =>
            {
                if (e == "My")
                    pipelineStarted++;
            };

            //act
            await pipeline.RunAsync(EventArgs.Empty);

            //assert
            pipelineStarted.Should().Be(1);
        }

        [Fact]
        public async Task RegisterPipelineSteps_RegisterSteps_StepsAndCtorParamsAreRegisteredRecursive()
        {
            await AssertRegisterPipelineStepsAsync<EventArgs>(p => { p.AddStep<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>(); });
        }

        [Fact]
        public async Task RegisterPipelineSteps_RegisterBlock_BlocksAreRegistered()
        {
            await AssertRegisterPipelineStepsAsync<EventArgs>(p => { p.AddBlock("MyBlock"); });
        }

        [Fact]
        public async Task LazyLoad_RegisterStepsInBlock_StepsAndCtorParamsAreRegisteredRecursively()
        {
            await AssertRegisterPipelineStepsAsync<EventArgs>(p => { p.AddBlock("MyBlock").AddStep<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>(); });
        }

        private async Task AssertRegisterPipelineStepsAsync<T>(Action<Pipeline<T>> addAction)
        {
            var pipeline = new Pipeline<T>(services =>
            {
                //register abstract types specifically
                services.AddTransient<IAbstract, ConcreteClass>();
                services.AddTransient<ClassThatTakesAnAbstractClassAsCtorParam>();
                //abstract dependency registrations are not overridden
                var pipelines = typeof(PipelineTests).Assembly.GetPipelineTypes().ToList();
                pipelines.ForEach(p => services.AddTransient(p));
                var pipelineSteps = typeof(PipelineTests).Assembly.GetPipelineStepTypes();
                pipelineSteps.ForEach(p => services.AddTransient(p));
            });
            addAction(pipeline);

            Func<Task> action = async () => await pipeline.RunAsync(default(T));

            await action.Should().NotThrowAsync().ConfigureAwait(false);
        }

        [Fact]
        public void Ctor_LazyLoadStepName_NameIsSetOnAdd()
        {
            var pipeline = new Pipeline<EventArgs>();
            pipeline.AddStep<SimpleStep>();

            pipeline.Tasks.Single().Name.Should().Be("Simple");
        }

        [Fact]
        public async Task Ctor_ArgsInheritanceHierarchy_StepsWithAncestorArgsCanBeUsedInPipeline()
        {
            var argsInit = new DescendantArgs();
            argsInit.AncestorUpdated.Should().BeFalse();
            argsInit.DescendantUpdated.Should().BeFalse();
            var pipeline = new Pipeline<DescendantArgs>(services =>
            {
                services.AddTransient<DescendantStep>();
            });

            //act
            pipeline.AddStep((args, ct) => new AncestorStep().RunAsync(args, ct));
            pipeline.AddStep<DescendantStep>();

            //assert
            var argsUpdated = await pipeline.RunAsync(argsInit, CancellationToken.None);
            argsInit.AncestorUpdated.Should().BeTrue();
            argsInit.DescendantUpdated.Should().BeTrue();
        }

        [Theory]
        [InlineData(Invoke.Parallel)]
        [InlineData(Invoke.Sequential)]
        public async Task RunAsync_TaskCancellation_PipelineIsCancelled(Invoke invoke)
        {
            var pipeline = new Pipeline<EventArgs>(invoke: invoke);

            var ts = new CancellationTokenSource();
            ts.Cancel();
            ts.IsCancellationRequested.Should().BeTrue();
            var counter = 0;
            var stepCount = 101;

            for (var i = 0; i < stepCount; i++)
                pipeline.AddStep((args, ct) => Task.FromResult(++counter));

            await pipeline.RunAsync(null, ts.Token);

            pipeline.Tasks.Count().Should().Be(stepCount);
            counter.Should().Be(0);
        }

        [Fact]
        public async Task DisplayName_DisplayNameIsNotSet_TypeNameNameIsUsed()
        {
            var pipeline = new Pipeline<EventArgs<int>>(services =>
            {
                services.AddSingleton<IncrementArgsStep>();
            });
            pipeline.AddStep<IncrementArgsStep>();
            string stepName = null;

            pipeline.Started += (name) =>
            {
                if (name.StartsWith("IncrementArgs", StringComparison.OrdinalIgnoreCase))
                    stepName = name;
            };

            await pipeline.RunAsync(new EventArgs<int>());

            stepName.Should().Be("IncrementArgs");
        }

        [Fact]
        public async Task RunAsync_AllInParallel_AllStepsAreRunInParallel()

        {
            var pipeline = new Pipeline<EventArgs>();
            var block = pipeline.AddBlock("ParallelBlock", Invoke.Parallel);

            var runCount = 101;

            for (var i = 0; i < runCount; i++)
                block.AddStep(async (args, ct) => await Task.Delay(TimeSpan.FromSeconds(5), ct));

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await pipeline.RunAsync(null);
            stopwatch.Stop();

            //total timespan should be close to 1 second since tasks were run in parallel
            stopwatch.Elapsed.Should().BeCloseTo(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(6));
        }

        [Fact]
        public async Task RunAsync_BlockWait_StepsAreRunInBlockOrder()
        {
            var pipeline = new Pipeline<EventArgs>();
            var task1Called = false;
            var task2Called = false;

            pipeline.AddBlock("1", async (args, ct) => { task1Called = true; await Task.Delay(TimeSpan.FromMilliseconds(200), ct); return 0; });
            pipeline.AddBlock("2", (args, ct) =>
            {
                if (task1Called == false)
                    throw new ArgumentException("Task 1 not called");
                if (task2Called)
                    throw new ArgumentException("Task 2 called already");
                task2Called = true;
                return Task.FromResult(0);
            });

            task1Called.Should().BeFalse();
            task2Called.Should().BeFalse();

            await pipeline.RunAsync(null);

            task1Called.Should().BeTrue();
            task2Called.Should().BeTrue();
        }

        [Fact]
        public async Task RunAsync_PassArgs_ArgsArePassedInPipeline()
        {
            var pipeline = new Pipeline<EventArgs<int>>(services =>
            {
                services.AddSingleton<IncrementArgsStep>();
            });
            pipeline.AddStep<IncrementArgsStep>();
            pipeline.AddStep<IncrementArgsStep>();
            pipeline.AddBlock("1").AddStep<IncrementArgsStep>();
            pipeline.AddStep<IncrementArgsStep>();
            pipeline.AddBlock("2").AddStep<IncrementArgsStep>();

            var resultArgs = new EventArgs<int>();
            await pipeline.RunAsync(resultArgs, CancellationToken.None);
            resultArgs.Value.Should().Be(5);
        }

        [Fact]
        public void Add_AddGenericSteps_StepsAreAdded()
        {
            var block = new Pipeline<EventArgs>();
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
                block.AddStep((args, ct) => { });

            block.Tasks.Count().Should().Be(taskCount);
        }

        [Theory]
        [InlineData(101, Invoke.Parallel)]
        [InlineData(10, Invoke.Sequential)]
        public async Task BlockInvokeStyle_Sequence_TasksAreRunAccordingToInvokeStyle(int stepCount, Invoke invoke)
        {
            var block = new Pipeline<EventArgs<int>>(invoke: invoke);
            var @lock = new Lock();

            for (var i = 0; i < stepCount; i++)
                block.AddStep(async (args, ct) =>
                {
                    @lock.Enter();
                    args.Value += 1;
                    await Task.Delay(TimeSpan.FromSeconds(100), CancellationToken.None);

                    @lock.Exit();
                });

            var argsParam = new EventArgs<int>();

            //act
            await block.RunAsync(argsParam);
            //assert
            argsParam.Value.Should().Be(stepCount);
        }

        [Fact]
        public async Task RunAsync_Events_StartAndEndEventsAreRaised()
        {
            //arrange
            var started = new List<string>();
            var logEntries = new List<string>();
            var ended = new List<string>();


            var pipeline = new Pipeline<EventArgs<int>>(services =>
            {
                services.AddSingleton<IncrementArgsStep>();
            });
            pipeline.AddBlock().AddStep<IncrementArgsStep>();
            pipeline.AddStep("StepInline", (args, ct) => { });

            pipeline.Started += started.Add;
            pipeline.Ended += (name, e) => ended.Add(name);

            //act
            await pipeline.RunAsync(new EventArgs<int>());

            //assert
            started.Count.Should().Be(4);
            logEntries.Count.Should().Be(0);
            ended.Count.Should().Be(4);

            started.Count(s => s == "Pipeline<EventArgs<Int32>>").Should().Be(1);
            started.Count(s => s == "Block 0").Should().Be(1);
            started.Count(s => s == "IncrementArgs").Should().Be(1);
            started.Count(s => s == "StepInline").Should().Be(1);

            ended.Count(s => s == "Pipeline<EventArgs<Int32>>").Should().Be(1);
            ended.Count(s => s == "Block 0").Should().Be(1);
            ended.Count(s => s == "IncrementArgs").Should().Be(1);
            ended.Count(s => s == "StepInline").Should().Be(1);
        }
    }
}
