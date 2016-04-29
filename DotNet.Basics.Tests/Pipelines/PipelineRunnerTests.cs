using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Ioc;
using DotNet.Basics.Pipelines;
using DotNet.Basics.Sys;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Pipelines
{
    [TestFixture]
    public class PipelineRunnerTests
    {
        private IocContainer _container;

        [SetUp]
        public void SetUp()
        {
            _container = new IocContainer();
            _container.Register<ClassThatIncrementArgsDependOn>();
        }

        [Test]
        public async Task Ctor_Logger_LoggerIsLoggedTo()
        {
            var logger = new InMemLogger();
            const string message = "MyMessage";
            var pipeline = new Pipeline();

            pipeline.AddBlock((args, l) => l.LogInformation(message),
                            (args, l) => l.LogInformation(message),
                            (args, l) => l.LogInformation(message)
            );

            await Task.Delay(100.MilliSeconds()).ConfigureAwait(false);
            logger.GetLogs(LogLevel.Information).Count.Should().Be(0);//ensure task isn't started until runner starts

            var runner = new PipelineRunner(_container, logger);

            await runner.RunAsync(pipeline).ConfigureAwait(false);

            logger.GetLogs(LogLevel.Information).All(e => e.Message == message).Should().BeTrue();

        }

        [Test]
        public async Task DisplayName_DisplayNameIsSet_DisplayNameIsUsed()
        {
            var pipeline = new Pipeline<EventArgs>();
            pipeline.AddBlock().AddStep<PipelineStepWithDisplayNameSetStep>();
            var runner = new PipelineRunner(_container);
            string stepName = null;

            runner.StepStarting += delegate (string name) { stepName = name; };

            var result = await runner.RunAsync(pipeline).ConfigureAwait(false);

            stepName.Should().Be("MyDisplayName");
        }

        [Test]
        public async Task DisplayName_DisplayNameIsNotSet_TypeNameNameIsUsed()
        {
            var pipeline = new Pipeline<EventArgs<int>>();
            pipeline.AddBlock().AddStep<IncrementArgsStep>();
            var runner = new PipelineRunner(_container);
            string stepName = null;

            runner.StepStarting += delegate (string name) { stepName = name; };

            var result = await runner.RunAsync(pipeline).ConfigureAwait(false);

            stepName.Should().Be("MyIncrementArgsStep");
        }

        [Test]
        public async Task RunAsync_PipelineSpecifiedAsGenericParameter_PipelineIsRun()
        {
            var runner = new PipelineRunner(_container);
            var result = await runner.RunAsync<SimplePipeline, EventArgs<int>>().ConfigureAwait(false);
            result.Args.Value.Should().Be(1);
        }

        [Test]
        public async Task RunAsync_AllInParallel_AllStepsAreRunInParallel()
        {
            var pipeline = new Pipeline<EventArgs<int>>();
            pipeline.AddBlock(async (args, logger) => await Task.Delay(TimeSpan.FromSeconds(1)),
                              async (args, logger) => await Task.Delay(TimeSpan.FromSeconds(1)),
                              async (args, logger) => await Task.Delay(TimeSpan.FromSeconds(1)),
                              async (args, logger) => await Task.Delay(TimeSpan.FromSeconds(1)),
                              async (args, logger) => await Task.Delay(TimeSpan.FromSeconds(1)),
                              async (args, logger) => await Task.Delay(TimeSpan.FromSeconds(1)),
                              async (args, logger) => await Task.Delay(TimeSpan.FromSeconds(1)),
                              async (args, logger) => await Task.Delay(TimeSpan.FromSeconds(1)),
                              async (args, logger) => await Task.Delay(TimeSpan.FromSeconds(1)),
                              async (args, logger) => await Task.Delay(TimeSpan.FromSeconds(1)));

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await new PipelineRunner(_container).RunAsync(pipeline).ConfigureAwait(false);
            stopwatch.Stop();

            //total timespan should be close to 1 second since tasks were run in parallel
            stopwatch.Elapsed.Should().BeCloseTo(TimeSpan.FromSeconds(1), 2000);
        }

        [Test]
        public async Task RunAsync_BlockWait_StepsAreRunInBlockOrder()
        {
            var task1Called = false;
            var task2Called = false;
            var pipeline = new Pipeline<EventArgs<int>>();
            pipeline.AddBlock(async (args, logger) => { task1Called = true; await Task.Delay(TimeSpan.FromMilliseconds(200)); });
            pipeline.AddBlock(async (args, logger) =>
            {
                await Task.Delay(0).ConfigureAwait(false);
                if (task1Called == false)
                    throw new ArgumentException("Task 1 not called");
                if (task2Called)
                    throw new ArgumentException("Task 2 called already");
                task2Called = true;
            });

            task1Called.Should().BeFalse();
            task2Called.Should().BeFalse();

            await new PipelineRunner(_container).RunAsync(pipeline).ConfigureAwait(false);

            task1Called.Should().BeTrue();
            task2Called.Should().BeTrue();
        }

        [Test]
        [TestCase(LogLevel.Debug)]
        [TestCase(LogLevel.Verbose)]
        [TestCase(LogLevel.Information)]
        [TestCase(LogLevel.Warning)]
        [TestCase(LogLevel.Error)]
        [TestCase(LogLevel.Critical)]
        public async Task RunAsync_Result_FinishedWitEntryLogged(LogLevel logLevel)
        {
            var pipeline = new Pipeline<EventArgs<int>>();
            pipeline.AddBlock((args, log) => { log.Log("Entry logged", logLevel); });
            var logger = new InMemLogger();

            var result = await new PipelineRunner().RunAsync(pipeline, logger: logger).ConfigureAwait(false);

            foreach (LogLevel value in Enum.GetValues(typeof(LogLevel)))
            {
                var found = logger.GetLogs(value).Any();

                if (logLevel == LogLevel.Verbose)
                    logger.GetLogs(logLevel).Count.Should().Be(7);
                else if (value == LogLevel.Verbose)
                    logger.GetLogs(value).Count.Should().Be(6);

                if (value == logLevel)
                    found.Should().BeTrue(value.ToName());
                else if (value != LogLevel.Verbose)
                    found.Should().BeFalse(value.ToName());


                if (logLevel < LogLevel.Error && logLevel != LogLevel.None)
                    result.Success.Should().BeTrue($"{logLevel.ToName()} success: {result.Success}");
                else
                    result.Success.Should().BeFalse($"{logLevel.ToName()} success: {result.Success}");
            }
        }

        [Test]
        public async Task RunAsync_PassArgs_ArgsArePassedInPipeline()
        {
            var pipeline = new Pipeline<EventArgs<int>>();
            pipeline.AddBlock().AddStep<IncrementArgsStep>();
            pipeline.AddBlock().AddStep<IncrementArgsStep>();
            pipeline.AddBlock().AddStep<IncrementArgsStep>();
            pipeline.AddBlock().AddStep<IncrementArgsStep>();
            pipeline.AddBlock().AddStep<IncrementArgsStep>();

            var result = await new PipelineRunner(_container).RunAsync(pipeline).ConfigureAwait(false);
            result.Args.Value.Should().Be(5);
        }

        [Test]
        public async Task RunAsync_Events_StartAndEndEventsAreRaised()
        {
            //arrange
            var pipeline = new Pipeline<EventArgs<int>>();
            pipeline.AddBlock().AddStep<IncrementArgsStep>();
            var runner = new PipelineRunner(_container);
            string pipelineStarting = string.Empty;
            string pipelineEnded = string.Empty;
            string blockStarting = string.Empty;
            string blockEnded = string.Empty;
            string stepStarting = string.Empty;
            string stepEnded = string.Empty;

            runner.PipelineStarting += delegate (string name) { pipelineStarting = name; };
            runner.PipelineEnded += delegate (string name) { pipelineEnded = name; };
            runner.BlockStarting += delegate (string name) { blockStarting = name; };
            runner.BlockEnded += delegate (string name) { blockEnded = name; };
            runner.StepStarting += delegate (string name) { stepStarting = name; };
            runner.StepEnded += delegate (string name) { stepEnded = name; };

            //act
            await runner.RunAsync(pipeline).ConfigureAwait(false);

            //assert
            pipelineStarting.Should().Be("Pipeline`1", nameof(runner.PipelineStarting));
            pipelineEnded.Should().Be("Pipeline`1", nameof(runner.PipelineEnded));
            blockStarting.Should().Be("Block 0", nameof(runner.BlockStarting));
            blockEnded.Should().Be("Block 0", nameof(runner.BlockEnded));
            stepStarting.Should().Be("MyIncrementArgsStep", nameof(runner.StepStarting));
            stepEnded.Should().Be("MyIncrementArgsStep", nameof(runner.StepEnded));
        }

    }
}
