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
    public class TaskPipelineRunnerTests
    {
        private IIocContainer _container;

        [SetUp]
        public void SetUp()
        {
            _container = new IocContainer();
            _container.BindType<ClassThatIncrementArgsDependOn>();
        }

        [Test]
        public async Task DisplayName_DisplayNameIsSet_DisplayNameIsUsed()
        {
            var result = await TaskPipelineRunner.RunAsync<EventArgs>(pipeline =>
            {
                pipeline.AddBlock().AddStep<TaskStepWithDisplayNameSetStep>();
            }, container: _container).ConfigureAwait(false);

            result.Profile.BlockProfiles.Single().StepProfiles.Single().Name.Should().Be("MyDisplayName");
        }

        [Test]
        public async Task DisplayName_DisplayNameIsNotSet_TypeNameNameIsUsed()
        {
            var result = await TaskPipelineRunner.RunAsync<TEventArgs<int>>(pipeline =>
            {
                pipeline.AddBlock().AddStep<IncrementArgsStep>();
            }, container: _container).ConfigureAwait(false);

            result.Profile.BlockProfiles.Single().StepProfiles.Single().Name.Should().Be("MyIncrementArgsStep");
        }


        [Test]
        public async Task RunAsync_ImplicitPipeline_PipelineIsRun()
        {
            var logger = new InMemDiagnostics();
            var result = await TaskPipelineRunner.RunAsync<TEventArgs<int>>(pipeline =>
            {
                pipeline.AddBlock().AddStep<IncrementArgsStep>();
                pipeline.AddBlock().AddStep<IncrementArgsStep>();
            }, logger: logger, container: _container).ConfigureAwait(false);
            result.Args.Value.Should().Be(2);
            logger.GetLogs(LogLevel.Info).Count.Should().Be(2);
        }

        [Test]
        public async Task RunAsync_BlockName_BlockNamesAreInStatus()
        {
            const string blockName1 = "MyBLock01";
            const string blockName2 = "MyBLock02";

            var result = await TaskPipelineRunner.RunAsync<TEventArgs<int>>(pipeline =>
            {
                pipeline.AddBlock(blockName1).AddStep<IncrementArgsStep>();
                pipeline.AddBlock(blockName2).AddStep<IncrementArgsStep>();
            }, container: _container).ConfigureAwait(false);

            result.Profile.BlockProfiles.Count.Should().Be(2);
            result.Profile.BlockProfiles.First().Name.Should().Be(blockName1);
            result.Profile.BlockProfiles.Last().Name.Should().Be(blockName2);
        }
        [Test]
        public async Task RunAsync_PipelineSpecifiedAsGenericParameter_PipelineIsRun()
        {
            var runner = new TaskPipelineRunner(_container);
            var result = await runner.RunAsync<SimplePipeline, TEventArgs<int>>().ConfigureAwait(false);
            result.Args.Value.Should().Be(1);
        }

        [Test]
        public async Task RunAsync_AllInParallel_AllStepsAreRunInParallel()
        {
            var pipeline = new TaskPipeline<TEventArgs<int>>();
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
            await new TaskPipelineRunner(_container).RunAsync(pipeline).ConfigureAwait(false);
            stopwatch.Stop();

            //total timespan should be close to 1 second since tasks were run in parallel
            stopwatch.Elapsed.Should().BeCloseTo(TimeSpan.FromSeconds(1), 2000);
        }

        [Test]
        public async Task RunAsync_BlockWait_StepsAreRunInBlockOrder()
        {
            var task1Called = false;
            var task2Called = false;
            var pipeline = new TaskPipeline<TEventArgs<int>>();
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

            await new TaskPipelineRunner(_container).RunAsync(pipeline).ConfigureAwait(false);

            task1Called.Should().BeTrue();
            task2Called.Should().BeTrue();
        }

        [Test]
        public async Task RunAsync_Result_FinishedWithErrors()
        {
            var pipeline = new TaskPipeline<TEventArgs<int>>();
            pipeline.AddBlock(async (args, log) => { log.Log("This is error logged", LogLevel.Error); });
            var logger = new InMemDiagnostics();

            var result = await new TaskPipelineRunner().RunAsync(pipeline, logger: logger).ConfigureAwait(false);

            logger.GetLogs(LogLevel.Error).Any().Should().BeTrue();
            logger.GetLogs(LogLevel.Warning).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Info).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Debug).Any().Should().BeFalse();
            logger.Get<ProfileEntry>().Any().Should().BeTrue();
        }
        [Test]
        public async Task RunAsync_Result_FinishedWithWarnings()
        {
            var pipeline = new TaskPipeline<TEventArgs<int>>();
            pipeline.AddBlock(async (args, log) => { log.Log("This is warning logged", LogLevel.Warning); });
            var logger = new InMemDiagnostics();

            var result = await new TaskPipelineRunner().RunAsync(pipeline, logger: logger).ConfigureAwait(false);

            logger.GetLogs(LogLevel.Error).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Warning).Any().Should().BeTrue();
            logger.GetLogs(LogLevel.Info).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Debug).Any().Should().BeFalse();
            logger.Get<ProfileEntry>().Any().Should().BeTrue();
        }

        [Test]
        public async Task RunAsync_Result_FinishedWithInfos()
        {
            var pipeline = new TaskPipeline<TEventArgs<int>>();
            pipeline.AddBlock(async (args, log) => { log.Log("This is info logged"); });
            var logger = new InMemDiagnostics();

            var result = await new TaskPipelineRunner().RunAsync(pipeline, logger: logger).ConfigureAwait(false);

            logger.GetLogs(LogLevel.Error).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Warning).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Info).Any().Should().BeTrue();
            logger.GetLogs(LogLevel.Debug).Any().Should().BeFalse();
            logger.Get<ProfileEntry>().Any().Should().BeTrue();
        }

        [Test]
        public async Task RunAsync_Result_FinishedWithDebugs()
        {
            var pipeline = new TaskPipeline<TEventArgs<int>>();
            pipeline.AddBlock(async (args, log) => { log.Log("This is debug logged", LogLevel.Debug); });
            var logger = new InMemDiagnostics();

            var result = await new TaskPipelineRunner().RunAsync(pipeline, logger: logger).ConfigureAwait(false);

            logger.GetLogs(LogLevel.Error).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Warning).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Info).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Debug).Any().Should().BeTrue();
            logger.Get<ProfileEntry>().Any().Should().BeTrue();
        }
        [Test]
        public async Task RunAsync_Result_FinishedWithProfiles()
        {
            var pipeline = new TaskPipeline<TEventArgs<int>>();
            pipeline.AddBlock(async (args, log) => { log.Profile("", TimeSpan.FromSeconds(1)); });
            var logger = new InMemDiagnostics();

            var result = await new TaskPipelineRunner().RunAsync(pipeline, logger: logger).ConfigureAwait(false);

            logger.GetLogs(LogLevel.Error).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Warning).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Info).Any().Should().BeFalse();
            logger.GetLogs(LogLevel.Debug).Any().Should().BeFalse();
            logger.Get<ProfileEntry>().Any().Should().BeTrue();
        }

        [Test]
        public async Task RunAsync_PassArgs_ArgsArePassedInPipeline()
        {
            var pipeline = new TaskPipeline<TEventArgs<int>>();
            pipeline.AddBlock().AddStep<IncrementArgsStep>();
            pipeline.AddBlock().AddStep<IncrementArgsStep>();
            pipeline.AddBlock().AddStep<IncrementArgsStep>();
            pipeline.AddBlock().AddStep<IncrementArgsStep>();
            pipeline.AddBlock().AddStep<IncrementArgsStep>();

            var result = await new TaskPipelineRunner(_container).RunAsync(pipeline).ConfigureAwait(false);
            result.Args.Value.Should().Be(5);
        }
        
        [Test]
        public async Task RunAsync_Events_StartAndEndEventsAreRaised()
        {
            //arrange
            var pipeline = new TaskPipeline<TEventArgs<int>>();
            pipeline.AddBlock().AddStep<IncrementArgsStep>();
            var runner = new TaskPipelineRunner(_container);
            string pipelineStarting = string.Empty;
            string pipelineEnded = string.Empty;
            string blockStarting = string.Empty;
            string blockEnded = string.Empty;
            string stepStarting = string.Empty;
            string stepEnded = string.Empty;

            runner.PipelineStarting += delegate (Profile stats) { pipelineStarting = stats.ToString(); };
            runner.PipelineEnded += delegate (Profile stats) { pipelineEnded = stats.ToString(); };
            runner.BlockStarting += delegate (Profile stats) { blockStarting = stats.ToString(); };
            runner.BlockEnded += delegate (Profile stats) { blockEnded = stats.ToString(); };
            runner.StepStarting += delegate (Profile stats) { stepStarting = stats.ToString(); };
            runner.StepEnded += delegate (Profile stats) { stepEnded = stats.ToString(); };

            //act
            await runner.RunAsync(pipeline).ConfigureAwait(false);

            //assert
            pipelineStarting.Should().Contain("started");
            pipelineEnded.Should().Contain("finished");
            blockStarting.Should().Contain("started");
            blockEnded.Should().Contain("finished");
            stepStarting.Should().Contain("started");
            stepEnded.Should().Contain("finished");
        }

        [Test]
        public async Task RunAsync_LogProfile_ProfilesAreLoggedForStartAndEnd()
        {

            //arrange
            var pipeline = new TaskPipeline<TEventArgs<int>>();
            pipeline.AddBlock().AddStep<IncrementArgsStep>();
            var runner = new TaskPipelineRunner(_container);
            var logger = new InMemDiagnostics();

            //act
            var result = await runner.RunAsync(pipeline, logger: logger).ConfigureAwait(false);

            //assert
            //pipeline, block and step started and finished = 6 profile log entries
            logger.Get<ProfileEntry>().Count.Should().Be(6);
        }
    }
}
