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
        public async Task RunSync_AsSingleton_TaskIsRunAssingleton()
        {
            var taskId = "RunSync_AsSingleton_TaskIsRunAssingleton";
            var bgRunner = new BackgroundTaskRunner();

            var runCount = 0;
            var runTimes = 10;
            var runRange = Enumerable.Range(0, runTimes);
            var startedCount = 0;
            var notStartedCount = 0;
            
            TaskEndedReason notStartedReason = TaskEndedReason.AllGood;

            runTimes.Should().BeGreaterThan(3);//task needs to run at least some times to ensure singleton

            bgRunner.TaskStarted += (args) =>
            {
                startedCount++;
            };
            bgRunner.TaskEnded += (args) =>
            {
                if (args.Reason == TaskEndedReason.AlreadyStarted)
                {
                    notStartedCount++;
                    notStartedReason = args.Reason;
                }
            };

            runRange.ForEach(i => bgRunner.StartAsSingleton(taskId, async rid =>
            {
                runCount++;
                await Task.Delay(250.Milliseconds()).ConfigureAwait(false);
            }));

            while (bgRunner.IsRunning(taskId))
                await Task.Delay(100.Milliseconds()).ConfigureAwait(false);

            runCount.Should().Be(1, "run count");
            startedCount.Should().Be(1);
            notStartedCount.Should().Be(runTimes - 1, "not started count");
            notStartedReason.Should().Be(TaskEndedReason.AlreadyStarted);
        }


        [Test]
        public async Task RunSync_GetExceptionsFromBgTasks_ExceptionsAreBubbled()
        {
            var taskId = "RunSync_GetExceptionsFromBgTasks_ExceptionsAreBubbled";

            await AssertTaskAsync(taskId, bgRunner =>
            {
                bgRunner.Start(taskId, (Action<string>)(rid =>
                     {
                         throw new ArgumentNullException();
                     }));
            }).ConfigureAwait(false);
        }
        [Test]
        public async Task RunAsync_GetExceptionsFromBgTasks_ExceptionsAreBubbled()
        {
            var taskId = "RunAsync_GetExceptionsFromBgTasks_ExceptionsAreBubbled";

            await AssertTaskAsync(taskId, bgRunner =>
             {
                 bgRunner.Start(taskId, rid =>
                 {
                     throw new ArgumentNullException();
                 });
             }).ConfigureAwait(false);
        }

        private async Task AssertTaskAsync(string taskId, Action<BackgroundTaskRunner> startTaskCallback)
        {
            var bgRunner = new BackgroundTaskRunner();
            bool taskRan = false;
            Exception lastException = null;
            string lastTaskId = null;
            string lastRunId = null;

            bool taskEnded = false;

            bgRunner.TaskStarted += (args) =>
             {
                 taskRan = true;
                 lastTaskId = args.TaskId;
                 lastRunId = args.RunId;
             };
            bgRunner.TaskEnded += (args) =>
               {
                   lastException = args.Exception;
                   taskEnded = true;
               };

            startTaskCallback(bgRunner);

            while (taskEnded == false)
                await Task.Delay(50.Milliseconds(), CancellationToken.None).ConfigureAwait(false);

            taskRan.Should().BeTrue();
            lastTaskId.Should().Be(taskId);
            lastRunId.Should().NotBeNull();
            lastException.Should().BeOfType<ArgumentNullException>();
        }
    }
}
