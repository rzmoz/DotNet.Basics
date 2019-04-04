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
        public async Task Started_Events_EventsIsOnlyTriggeredOnce()
        {
            var pipelineName = "MyPipeline";
            var pipelineStarted = 0;


            var pipeline = new Pipeline<EventArgs>(pipelineName);
            var block = pipeline.AddBlock();

            block.AddStep((args, log, ct) => { });

            pipeline.Started += (e) =>
            {
                if (e == pipelineName)
                    pipelineStarted++;
            };

            //act
            await pipeline.RunAsync(EventArgs.Empty).ConfigureAwait(false);

            //assert
            pipelineStarted.Should().Be(1);
        }

        [Fact]
        public async Task MessageLogged_LogFromSteps_MessagesHaveContext()
        {
            var pipelineReceived = string.Empty;
            var message = "Hello World!";

            var pipelineName = "MyPipeline";
            var pipeline = new Pipeline<EventArgs>(pipelineName);
            var blockName = "MyBlock";
            var block = pipeline.AddBlock(blockName);
            var stepName = "MyStep";
            block.AddStep(stepName, (args, log, ct) => log.Debug(message));

            pipeline.MessageLogged += (lvl, msg, e) => pipelineReceived = msg;

            //act
            await pipeline.RunAsync(EventArgs.Empty).ConfigureAwait(false);

            //assert
            pipelineReceived.Should().Be($"{pipelineName} / {blockName} / {stepName} / {message}");
        }

        [Fact]
        public void RegisterPipelineSteps_RegisterSteps_StepsAndCtorParamsAreRegisteredRecursive()
        {
            AssertRegisterPipelineSteps<EventArgs>(p => { p.AddStep<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>(); });
        }

        [Fact]
        public void RegisterPipelineSteps_RegisterBlock_BlocksAreRegistered()
        {
            AssertRegisterPipelineSteps<EventArgs>(p => { p.AddBlock("MyBlock"); });
        }

        [Fact]
        public void LazyLoad_RegisterStepsInBlock_StepsAndCtorParamsAreRegisteredRecursively()
        {
            AssertRegisterPipelineSteps<EventArgs>(p => { p.AddBlock("MyBlock").AddStep<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>(); });
        }

        private void AssertRegisterPipelineSteps<T>(Action<Pipeline<T>> addAction)
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

            Func<Task> action = async () => await pipeline.RunAsync(default(T)).ConfigureAwait(false);

            action.Should().NotThrow();
        }

        [Fact]
        public void Ctor_LazyLoadStepName_NameIsSetOnAdd()
        {
            var pipeline = new Pipeline<EventArgs>();
            pipeline.AddStep<SimpleStep>();

            pipeline.Tasks.Single().Name.Should().Be(nameof(SimpleStep));
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
            pipeline.AddStep((args, log, ct) => new AncestorStep().RunAsync(args, ct));
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
            var pipeline = new Pipeline<EventArgs>(invoke: invoke);

            var ts = new CancellationTokenSource();
            ts.Cancel();
            ts.IsCancellationRequested.Should().BeTrue();
            var counter = 0;
            var stepCount = 101;

            for (var i = 0; i < stepCount; i++)
                pipeline.AddStep((args, log, ct) => Task.FromResult(++counter));

            await pipeline.RunAsync(null, ts.Token).ConfigureAwait(false);

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
                if (name.EndsWith("step", StringComparison.OrdinalIgnoreCase))
                    stepName = name;
            };

            await pipeline.RunAsync(new EventArgs<int>()).ConfigureAwait(false);

            stepName.Should().Be(nameof(IncrementArgsStep));
        }

        [Fact]
        public async Task RunAsync_AllInParallel_AllStepsAreRunInParallel()
        {
            var pipeline = new Pipeline<EventArgs>();
            var block = pipeline.AddBlock("ParallelBlock", Invoke.Parallel);

            var runCount = 101;

            for (var i = 0; i < runCount; i++)
                block.AddStep(async (args, log, ct) => await Task.Delay(TimeSpan.FromSeconds(1), ct));

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await pipeline.RunAsync(null).ConfigureAwait(false);
            stopwatch.Stop();

            //total timespan should be close to 1 second since tasks were run in parallel
            stopwatch.Elapsed.Should().BeCloseTo(TimeSpan.FromSeconds(6), 5000);
        }

        [Fact]
        public async Task RunAsync_BlockWait_StepsAreRunInBlockOrder()
        {
            var pipeline = new Pipeline<EventArgs>();
            var task1Called = false;
            var task2Called = false;

            pipeline.AddBlock("1", async (args, log, ct) => { task1Called = true; await Task.Delay(TimeSpan.FromMilliseconds(200), ct); });
            pipeline.AddBlock("2", (args, log, ct) =>
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

            await pipeline.RunAsync(null).ConfigureAwait(false);

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
            await pipeline.RunAsync(resultArgs, CancellationToken.None).ConfigureAwait(false);
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
                block.AddStep((args, log, ct) => { });

            block.Tasks.Count().Should().Be(taskCount);
        }

        [Theory]
        [InlineData(101, Invoke.Parallel)]
        [InlineData(10, Invoke.Sequential)]
        public async Task BlockInvokeStyle_Sequence_TasksAreRunAccordingToInvokeStyle(int stepCount, Invoke invoke)
        {
            var block = new Pipeline<EventArgs<int>>(invoke: invoke);
            int lockFlag = 0;

            var raceConditionEncountered = 0;
            for (var i = 0; i < stepCount; i++)
                block.AddStep(async (args, log, ct) =>
                {
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

            await block.RunAsync(argsParam, CancellationToken.None).ConfigureAwait(false);

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
            var started = new List<string>();
            var logEntries = new List<string>();
            var ended = new List<string>();


            var pipeline = new Pipeline<EventArgs<int>>(services =>
            {
                services.AddSingleton<IncrementArgsStep>();
            });
            pipeline.AddBlock().AddStep<IncrementArgsStep>();
            pipeline.AddStep("StepInline", (args, log, ct) => { });

            pipeline.Started += started.Add;
            pipeline.Ended += (name, e) => ended.Add(name);

            //act
            await pipeline.RunAsync(new EventArgs<int>()).ConfigureAwait(false);

            //assert
            started.Count.Should().Be(4);
            logEntries.Count.Should().Be(0);
            ended.Count.Should().Be(4);

            started.Count(s => s == "Pipeline<EventArgs<Int32>>").Should().Be(1);
            started.Count(s => s == "Block 0").Should().Be(1);
            started.Count(s => s == "IncrementArgsStep").Should().Be(1);
            started.Count(s => s == "StepInline").Should().Be(1);

            ended.Count(s => s == "Pipeline<EventArgs<Int32>>").Should().Be(1);
            ended.Count(s => s == "Block 0").Should().Be(1);
            ended.Count(s => s == "IncrementArgsStep").Should().Be(1);
            ended.Count(s => s == "StepInline").Should().Be(1);
        }
    }
}
