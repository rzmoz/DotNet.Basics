using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Collections;
using DotNet.Basics.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Tasks
{
    [TestFixture]
    public class BackgroundTaskRunnerTests
    {
        [Test]
        public async Task IsRunning_AsSingleton_TaskRunningIsdetected()
        {
            var runner = new TaskRunner();
            var taskId = "IsRunning_AsSingleton_TaskRunningIsdetected";

            var ctSource = new CancellationTokenSource();

            var started = await runner.TryStartAsync(taskId, async rid =>
            {
                await Task.Delay(60.Minutes(), ctSource.Token).ConfigureAwait(false);
            }, RunMode.Singleton | RunMode.Background).ConfigureAwait(false);

            //ensure that task is started
            await Task.Delay(500.Milliseconds(), CancellationToken.None).ConfigureAwait(false);

            //act
            var isRunningdetected = runner.IsRunning(taskId);
            ctSource.Cancel();

            //assert
            started.Should().BeTrue("started");
            isRunningdetected.Should().BeTrue("IsRunning detected");
        }

        [Test]
        public async Task RunSync_AsSingleton_TaskIsRunAssingleton()
        {
            var taskId = "RunSync_AsSingleton_TaskIsRunAssingleton";
            var runner = new TaskRunner();
            var runCount = 0;
            var runTimes = 10;
            var runRange = Enumerable.Range(0, runTimes);
            var startedCount = 0;
            var notStartedCount = 0;

            runTimes.Should().BeGreaterThan(3);//task needs to run at least some times to ensure singleton

            runner.TaskStarted += (args) =>
            {
                startedCount++;
            };
            runner.TaskEnded += (args) =>
            {
                if (args.Reason == TaskEndedReason.AlreadyStarted)
                {
                    notStartedCount++;
                }
            };

            runRange.ParallelForEach(i => runner.TryStart(taskId, async rid =>
            {
                runCount++;
                await Task.Delay(500.Milliseconds()).ConfigureAwait(false);
            }, RunMode.Singleton | RunMode.Background));

            await runner.TillTaskIsFinished(taskId).ConfigureAwait(false);

            runCount.Should().Be(1, "run count");
            startedCount.Should().Be(1, "task started");
            notStartedCount.Should().Be(runTimes - 1, "not started count");
        }


        [Test]
        public async Task RunSync_GetExceptionsFromBgTasks_ExceptionsAreBubbled()
        {
            var taskId = "RunSync_GetExceptionsFromBgTasks_ExceptionsAreBubbled";

            await AssertTaskAsync(taskId, runner =>
            {
                runner.TryStart(taskId, (Action<string>)(rid =>
                {
                    throw new ApplicationException(rid);
                }), RunMode.Background);
            }).ConfigureAwait(false);
        }
        [Test]
        public async Task RunAsync_GetExceptionsFromBgTasks_ExceptionsAreBubbled()
        {
            var taskId = "RunAsync_GetExceptionsFromBgTasks_ExceptionsAreBubbled";

            await AssertTaskAsync(taskId, bgRunner =>
             {
                 bgRunner.TryStart(taskId, rid =>
                 {
                     throw new ApplicationException(rid);
                 }, RunMode.Background);
             }).ConfigureAwait(false);
        }

        private async Task AssertTaskAsync(string taskId, Action<TaskRunner> startTaskCallback)
        {
            var bgRunner = new TaskRunner();
            bool taskRan = false;
            Exception lastException = null;
            string lastTaskId = null;
            string lastRunId = null;

            bgRunner.TaskStarted += (args) =>
             {
                 taskRan = true;
                 lastTaskId = args.TaskId;
                 lastRunId = args.RunId;
             };
            bgRunner.TaskEnded += (args) =>
               {
                   lastException = args.Exception;
               };

            startTaskCallback(bgRunner);

            await bgRunner.TillTaskIsFinished(taskId).ConfigureAwait(false);

            taskRan.Should().BeTrue();
            lastTaskId.Should().Be(taskId);
            lastRunId.Should().NotBeNull();
            lastException.Should().BeOfType<ApplicationException>();
        }
    }
}
