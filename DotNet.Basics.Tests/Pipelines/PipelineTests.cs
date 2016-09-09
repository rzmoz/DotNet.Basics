﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Ioc;
using DotNet.Basics.Pipelines;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Pipelines
{
    [TestFixture]
    public class PipelineTests
    {
        private SimpleContainer _container;

        [SetUp]
        public void SetUp()
        {
            _container = new SimpleContainer();
            _container.Register<ClassThatIncrementArgsDependOn>();
        }

        [Test]
        public async Task RunAsync_TaskCancellation_PipelineIsCancelled()
        {
            var pipeline = new Pipeline<EventArgs>(_container);

            var ts = new CancellationTokenSource();

            var counter = 0;
            var initialStepName = "cancellationStep";
            var additionalStepCount = 10;

            pipeline.AddBlock().AddStep(initialStepName, async (args, ct) =>
            {
                counter++;
                while (ct.IsCancellationRequested == false)
                    await Task.Delay(100.Milliseconds(), ct).ConfigureAwait(false);
            });

            for (var i = 0; i < additionalStepCount; i++)
                pipeline.AddBlock().AddStep(async (args, ct) => counter++);

            pipeline.SectionStarted += args =>
            {
                if (args.Name == initialStepName)
                    ts.Cancel();
            };

            var pipelineTask = pipeline.RunAsync(ts.Token);
            
            await Task.WhenAll(pipelineTask).ConfigureAwait(false);
            pipeline.Count().Should().Be(additionalStepCount + 1);
            counter.Should().Be(1);
        }


        [Test]
        public async Task DisplayName_DisplayNameIsSet_DisplayNameIsUsed()
        {
            var pipeline = new Pipeline<EventArgs>(_container);
            pipeline.AddBlock().AddStep<PipelineStepWithDisplayNameSetStep>();

            string stepName = null;

            pipeline.SectionStarted += (e) =>
            {
                if (e.Type == SectionType.Step)
                    stepName = e.Name;
            };

            await pipeline.RunAsync().ConfigureAwait(false);

            stepName.Should().Be("ThisStepHasCustomName");
        }

        [Test]
        public async Task DisplayName_DisplayNameIsNotSet_TypeNameNameIsUsed()
        {
            var pipeline = new Pipeline<EventArgs<int>>(_container);
            pipeline.AddBlock().AddStep<IncrementArgsStep>();
            string stepName = null;

            pipeline.SectionStarted += (e) =>
            {
                if (e.Type == SectionType.Step)
                    stepName = e.Name;
            };

            await pipeline.RunAsync().ConfigureAwait(false);

            stepName.Should().Be("IncrementArgsStep");
        }

        [Test]
        public async Task RunAsync_AllInParallel_AllStepsAreRunInParallel()
        {
            var pipeline = new Pipeline<EventArgs<int>>(_container);
            var block = pipeline.AddBlock();

            for (var i = 0; i < 10; i++)
                block.AddStep(async (args, ct) => await Task.Delay(TimeSpan.FromSeconds(1), ct));

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await pipeline.RunAsync().ConfigureAwait(false);
            stopwatch.Stop();

            //total timespan should be close to 1 second since tasks were run in parallel
            stopwatch.Elapsed.Should().BeCloseTo(TimeSpan.FromSeconds(1), 2000);
        }

        [Test]
        public async Task RunAsync_BlockWait_StepsAreRunInBlockOrder()
        {
            var pipeline = new Pipeline<EventArgs<int>>(_container);
            var task1Called = false;
            var task2Called = false;

            pipeline.AddBlock(async (args, ct) => { task1Called = true; await Task.Delay(TimeSpan.FromMilliseconds(200), ct); });
            pipeline.AddBlock((args, xct) =>
            {
                if (task1Called == false)
                    throw new ArgumentException("Task 1 not called");
                if (task2Called)
                    throw new ArgumentException("Task 2 called already");
                task2Called = true;
                return Task.CompletedTask;
            });

            task1Called.Should().BeFalse();
            task2Called.Should().BeFalse();

            await pipeline.RunAsync().ConfigureAwait(false);

            task1Called.Should().BeTrue();
            task2Called.Should().BeTrue();
        }


        [Test]
        public async Task RunAsync_PassArgs_ArgsArePassedInPipeline()
        {
            var pipeline = new Pipeline<EventArgs<int>>(_container);
            pipeline.AddBlock().AddStep<IncrementArgsStep>();
            pipeline.AddBlock().AddStep<IncrementArgsStep>();
            pipeline.AddBlock().AddStep<IncrementArgsStep>();
            pipeline.AddBlock().AddStep<IncrementArgsStep>();
            pipeline.AddBlock().AddStep<IncrementArgsStep>();

            var args = await pipeline.RunAsync().ConfigureAwait(false);
            args.Value.Should().Be(5);
        }

        [Test]
        public async Task RunAsync_Events_StartAndEndEventsAreRaised()
        {
            //arrange
            var pipeline = new Pipeline<EventArgs<int>>(_container);
            pipeline.AddBlock().AddStep<IncrementArgsStep>();
            string pipelineStarted = string.Empty;
            string pipelineEnded = string.Empty;
            string blockStarted = string.Empty;
            string blockEnded = string.Empty;
            string stepStarted = string.Empty;
            string stepEnded = string.Empty;

            pipeline.SectionStarted += args =>
            {
                switch (args.Type)
                {
                    case SectionType.Step:
                        stepStarted = args.Name;
                        break;
                    case SectionType.Block:
                        blockStarted = args.Name;
                        break;
                    case SectionType.Pipeline:
                        pipelineStarted = args.Name;
                        break;
                }
            };
            pipeline.SectionEnded += args =>
            {
                switch (args.Type)
                {
                    case SectionType.Step:
                        stepEnded = args.Name;
                        break;
                    case SectionType.Block:
                        blockEnded = args.Name;
                        break;
                    case SectionType.Pipeline:
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