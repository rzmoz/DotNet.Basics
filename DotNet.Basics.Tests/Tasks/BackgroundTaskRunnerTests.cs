using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            var startedCount = 0;
            var notStartedCount = 0;
            string notStartedReason = string.Empty;

            runTimes.Should().BeGreaterThan(3);//task needs to run at least some times to ensure singleton

            bgRunner.TaskStarted += (tid, rid) =>
            {
                startedCount++;
            };
            bgRunner.TaskNotStarted += (tid, rid, reason) =>
            {
                notStartedCount++;
                notStartedReason += reason;
            };

            var delay = 250.Milliseconds();

            Parallel.For(0, runTimes, i => bgRunner.StartAsSingleton(taskId, async rid =>
            {
                runCount++;
                await Task.Delay(delay).ConfigureAwait(false);
            }));

            await Task.Delay(delay + 250.Milliseconds()).ConfigureAwait(false);


            runCount.Should().Be(1, "run count");
            
            startedCount.Should().Be(1);
            notStartedCount.Should().Be(runTimes - 1, "not started count");
            notStartedReason.Should().Contain("already started");
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

            bgRunner.TaskStarted += (tid, rid) =>
             {
                 taskRan = true;
                 lastTaskId = tid;
                 lastRunId = rid;
             };
            bgRunner.TaskFailed += (tid, rid, e) =>
              {
                  lastException = e;
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
