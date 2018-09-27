using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;
using DotNet.Basics.Tasks.Pipelines;
using DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers;
using DotNet.Basics.Collections;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DotNet.Basics.Tests.Tasks.Pipelines
{
    public class PipelineTests
    {
        [Fact]
        public void RegisterPipelineSteps_RegisterSteps_StepsAndCtorParamsAreRegisteredRecursive()
        {
            AssertRegisterPipelineSteps<EventArgs>((p, sp) => { p.AddStep<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>(sp); });
        }

        [Fact]
        public void RegisterPipelineSteps_RegisterBlock_BlocksAreRegistered()
        {
            AssertRegisterPipelineSteps<EventArgs>((p, sp) => { p.AddBlock("MyBlock"); });
        }

        [Fact]
        public void LazyLoad_RegisterStepsInBlock_StepsAndCtorParamsAreRegisteredRecursive()
        {
            AssertRegisterPipelineSteps<EventArgs>((p, sp) => { p.AddBlock("MyBlock").AddStep<GenericThatTakesAnotherConcreteClassAsArgStep<EventArgs>>(sp); });
        }

        private void AssertRegisterPipelineSteps<T>(Action<Pipeline<T>, IServiceProvider> addAction) where T : class, new()
        {
            var provider = GetServiceProvider(services =>
            {
                //register abstract types specifically
                services.AddTransient<IAbstract, ConcreteClass>();
                services.AddTransient<ClassThatTakesAnAbstractClassAsCtorParam>();
                //abstract dependency registrations are not overridden
                var pipelines = typeof(PipelineTests).Assembly.GetPipelineTypes();
                pipelines.ForEach(services.AddTransient);
                var pipelineSteps = typeof(PipelineTests).Assembly.GetPipelineStepTypes();
                pipelineSteps.ForEach(services.AddTransient);
            });

            var pipeline = new Pipeline<T>();
            addAction(pipeline, provider);

            Func<Task> action = async () => await pipeline.RunAsync(CancellationToken.None).ConfigureAwait(false);

            action.Should().NotThrow();
        }

        [Theory]
        [InlineData(Invoke.Parallel)]
        [InlineData(Invoke.Sequential)]
        public async Task RunAsync_AddEntriesFromDerivedStepClass_EntriesAreCollectedAndAggregatedInFinalResult(Invoke invoke)
        {
            var provider = GetServiceProvider(services =>
            {
                services.AddSingleton<AddLogEntryStep>();
            });

            var pipeline = new Pipeline<EventArgs>(invoke);
            var count = 15;
            foreach (var i in Enumerable.Range(0, count))
                pipeline.AddStep<AddLogEntryStep>(provider);

            var result = await pipeline.RunAsync(CancellationToken.None).ConfigureAwait(false);

            result.Log.Count.Should().Be(count);
            result.Log.All(i => i.Message == nameof(AddLogEntryStep)).Should().BeTrue();
            result.Log.All(i => i.Exception == null).Should().BeTrue();
        }

        [Fact]
        public void Ctor_LazyLoadStepName_NameIsSetOnAdd()
        {
            var pipeline = new Pipeline<EventArgs>();
            pipeline.AddStep<AddLogEntryStep>(new ServiceCollection().BuildServiceProvider());

            pipeline.Tasks.Single().Name.Should().Be(nameof(AddLogEntryStep));
        }

        [Fact]
        public async Task Ctor_ArgsInheritanceHierarchy_StepsWithAncestorArgsCanBeUsedInPipeline()
        {
            var provider = GetServiceProvider(services =>
            {
                services.AddTransient<DescendantStep>();
            });

            var argsInit = new DescendantArgs();
            argsInit.AncestorUpdated.Should().BeFalse();
            argsInit.DescendantUpdated.Should().BeFalse();
            var pipeline = new Pipeline<DescendantArgs>();

            //act
            pipeline.AddStep((args, log, ct) => new AncestorStep().RunAsync(args, ct));
            pipeline.AddStep<DescendantStep>(provider);

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
            var pipeline = new Pipeline<EventArgs>(invoke);

            var ts = new CancellationTokenSource();
            ts.Cancel();
            ts.IsCancellationRequested.Should().BeTrue();
            var counter = 0;
            var stepCount = 101;

            for (var i = 0; i < stepCount; i++)
                pipeline.AddStep((args, log, ct) => Task.FromResult(++counter));

            await pipeline.RunAsync(ts.Token).ConfigureAwait(false);

            pipeline.Tasks.Count().Should().Be(stepCount);
            counter.Should().Be(0);
        }

        [Fact]
        public async Task DisplayName_DisplayNameIsNotSet_TypeNameNameIsUsed()
        {
            var provider = GetServiceProvider(services =>
            {
                services.AddSingleton<IncrementArgsStep>();
            });

            var pipeline = new Pipeline<EventArgs<int>>();
            pipeline.AddStep<IncrementArgsStep>(provider);
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
            var pipeline = new Pipeline<EventArgs>();
            var block = pipeline.AddBlock("ParallelBlock", Invoke.Parallel);

            var runCount = 101;

            for (var i = 0; i < runCount; i++)
                block.AddStep(async (args, log, ct) => await Task.Delay(TimeSpan.FromSeconds(1), ct));

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

            await pipeline.RunAsync(CancellationToken.None).ConfigureAwait(false);

            task1Called.Should().BeTrue();
            task2Called.Should().BeTrue();
        }

        [Fact]
        public async Task RunAsync_PassArgs_ArgsArePassedInPipeline()
        {
            var provider = GetServiceProvider(services =>
            {
                services.AddSingleton<IncrementArgsStep>();
            });

            var pipeline = new Pipeline<EventArgs<int>>();
            pipeline.AddStep<IncrementArgsStep>(provider);
            pipeline.AddStep<IncrementArgsStep>(provider);
            pipeline.AddBlock("1").AddStep<IncrementArgsStep>(provider);
            pipeline.AddStep<IncrementArgsStep>(provider);
            pipeline.AddBlock("2").AddStep<IncrementArgsStep>(provider);

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
                block.AddStep<ManagedTask<EventArgs>>(new ServiceCollection().BuildServiceProvider());

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
            var block = new Pipeline<EventArgs<int>>(invoke);
            int lockFlag = 0;

            var raceConditionEncountered = 0;
            for (var i = 0; i < stepCount; i++)
                block.AddStep(async (args, log, ct) =>
                {
                    log.Add(LogLevel.Error, i.ToString());

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

            result.Log.Count.Should().Be(stepCount);

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
            var provider = GetServiceProvider(services =>
            {
                services.AddSingleton<IncrementArgsStep>();
            });


            //arrange
            string pipelineStarted = string.Empty;
            string pipelineEnded = string.Empty;
            string blockStarted = string.Empty;
            string blockEnded = string.Empty;
            string stepStarted = string.Empty;
            string stepEnded = string.Empty;

            var pipeline = new Pipeline<EventArgs<int>>();
            pipeline.AddBlock().AddStep<IncrementArgsStep>(provider);

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

        private IServiceProvider GetServiceProvider(Action<IServiceCollection> addServices)
        {
            var services = new ServiceCollection();
            addServices(services);
            return services.BuildServiceProvider();
        }
    }
}
