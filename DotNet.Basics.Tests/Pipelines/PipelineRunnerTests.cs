using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Ioc;
using DotNet.Basics.Pipelines;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Pipelines
{
    [TestFixture]
    public class PipelineRunnerTests
    {
        private IIocContainer _container;

        [SetUp]
        public void SetUp()
        {
            _container = new IocContainer();
            _container.BindType<ClassThatIncrementArgsDependOn>();
        }

        [Test]
        public async Task Ctor_Logger_LoggerIsLoggedTo()
        {
            var logger = new InMemDiagnostics();
            const string message = "MyMessage";
            var pipeline = new Pipeline();

            pipeline.AddBlock(async (args, l) => l.Log(message),
                            async (args, l) => l.Log(message),
                            async (args, l) => l.Log(message)
            );

            var runner = new PipelineRunner(_container, logger);

            await runner.RunAsync(pipeline).ConfigureAwait(false);

            logger.GetLogs(LogLevel.Info).All(e => e.Message == message).Should().BeTrue();

        }

        [Test]
        public async Task DisplayName_DisplayNameIsSet_DisplayNameIsUsed()
        {
            var pipeline = new Pipeline<EventArgs>();
            pipeline.AddBlock().AddStep<PipelineStepWithDisplayNameSetStep>();
            var runner = new PipelineRunner(_container);
            string stepName = null;

            runner.StepStarting += delegate (string name, PipelineType type) { stepName = name; };

            var result = await runner.RunAsync(pipeline).ConfigureAwait(false);

            stepName.Should().Be("MyDisplayName");
        }

        [Test]
        public async Task DisplayName_DisplayNameIsNotSet_TypeNameNameIsUsed()
        {
            var pipeline = new Pipeline<TEventArgs<int>>();
            pipeline.AddBlock().AddStep<IncrementArgsStep>();
            var runner = new PipelineRunner(_container);
            string stepName = null;

            runner.StepStarting += delegate (string name, PipelineType type) { stepName = name; };

            var result = await runner.RunAsync(pipeline).ConfigureAwait(false);

            stepName.Should().Be("MyIncrementArgsStep");
        }

        [Test]
        public async Task RunAsync_PipelineSpecifiedAsGenericParameter_PipelineIsRun()
        {
            var runner = new PipelineRunner(_container);
            var result = await runner.RunAsync<SimplePipeline, TEventArgs<int>>().ConfigureAwait(false);
            result.Args.Value.Should().Be(1);
        }

        [Test]
        public async Task RunAsync_AllInParallel_AllStepsAreRunInParallel()
        {
            var pipeline = new Pipeline<TEventArgs<int>>();
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
            var pipeline = new Pipeline<TEventArgs<int>>();
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
        public async Task RunAsync_Result_FinishedWithErrors()
        {
            var pipeline = new Pipeline<TEventArgs<int>>();
            pipeline.AddBlock(async (args, log) => { log.Log("This is error logged", LogLevel.Error); });
            var logger = new InMemDiagnostics();

            var result = await new PipelineRunner().RunAsync(pipeline, logger: logger).ConfigureAwait(false);

            logger.GetLogs(LogLevel.Error).Any().Should().BeTrue();
            logger.GetLogs(LogLevel.Warning).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Info).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Debug).Count.Should().Be(6);

        }
        [Test]
        public async Task RunAsync_Result_FinishedWithWarnings()
        {
            var pipeline = new Pipeline<TEventArgs<int>>();
            pipeline.AddBlock(async (args, log) => { log.Log("This is warning logged", LogLevel.Warning); });
            var logger = new InMemDiagnostics();

            var result = await new PipelineRunner().RunAsync(pipeline, logger: logger).ConfigureAwait(false);

            logger.GetLogs(LogLevel.Error).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Warning).Any().Should().BeTrue();
            logger.GetLogs(LogLevel.Info).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Debug).Count.Should().Be(6);

        }

        [Test]
        public async Task RunAsync_Result_FinishedWithInfos()
        {
            var pipeline = new Pipeline<TEventArgs<int>>();
            pipeline.AddBlock(async (args, log) => { log.Log("This is info logged"); });
            var logger = new InMemDiagnostics();

            var result = await new PipelineRunner().RunAsync(pipeline, logger: logger).ConfigureAwait(false);

            logger.GetLogs(LogLevel.Error).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Warning).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Info).Any().Should().BeTrue();
            logger.GetLogs(LogLevel.Debug).Count.Should().Be(6);

        }

        [Test]
        public async Task RunAsync_Result_FinishedWithDebugs()
        {
            var pipeline = new Pipeline<TEventArgs<int>>();
            pipeline.AddBlock(async (args, log) => { log.Log("This is debug logged", LogLevel.Debug); });
            var logger = new InMemDiagnostics();

            var result = await new PipelineRunner().RunAsync(pipeline, logger: logger).ConfigureAwait(false);

            logger.GetLogs(LogLevel.Error).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Warning).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Info).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Debug).Count.Should().Be(7);
        }
        [Test]
        public async Task RunAsync_Result_FinishedWithProfiles()
        {
            var pipeline = new Pipeline<TEventArgs<int>>();
            pipeline.AddBlock(async (args, log) => { log.Profile("", TimeSpan.FromSeconds(1)); });
            var logger = new InMemDiagnostics();

            var result = await new PipelineRunner().RunAsync(pipeline, logger: logger).ConfigureAwait(false);

            logger.GetLogs(LogLevel.Error).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Warning).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Info).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Debug).Count.Should().Be(6);
        }

        [Test]
        public async Task RunAsync_PassArgs_ArgsArePassedInPipeline()
        {
            var pipeline = new Pipeline<TEventArgs<int>>();
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
            var pipeline = new Pipeline<TEventArgs<int>>();
            pipeline.AddBlock().AddStep<IncrementArgsStep>();
            var runner = new PipelineRunner(_container);
            string pipelineStarting = string.Empty;
            string pipelineEnded = string.Empty;
            string blockStarting = string.Empty;
            string blockEnded = string.Empty;
            string stepStarting = string.Empty;
            string stepEnded = string.Empty;

            runner.PipelineStarting += delegate (string name, PipelineType type) { pipelineStarting = name; };
            runner.PipelineEnded += delegate (string name, PipelineType type) { pipelineEnded = name; };
            runner.BlockStarting += delegate (string name, PipelineType type) { blockStarting = name; };
            runner.BlockEnded += delegate (string name, PipelineType type) { blockEnded = name; };
            runner.StepStarting += delegate (string name, PipelineType type) { stepStarting = name; };
            runner.StepEnded += delegate (string name, PipelineType type) { stepEnded = name; };

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
