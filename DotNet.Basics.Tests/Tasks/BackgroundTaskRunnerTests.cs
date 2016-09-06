using System;
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
        public async Task RunSync_GetExceptionsFromBgTasks_ExceptionsAreBubbled()
        {
            var taskId = "RunSync_GetExceptionsFromBgTasks_ExceptionsAreBubbled";

            await AssertTaskAsync(taskId, bgRunner =>
            {
                bgRunner.Start(taskId, () =>
                 {
                     throw new ArgumentNullException();

                 }, true);
            }).ConfigureAwait(false);
        }
        [Test]
        public async Task RunAsync_GetExceptionsFromBgTasks_ExceptionsAreBubbled()
        {
            var taskId = "RunAsync_GetExceptionsFromBgTasks_ExceptionsAreBubbled";

            await AssertTaskAsync(taskId, bgRunner =>
             {
                 bgRunner.Start(taskId, ct =>
                  {
                      throw new ArgumentNullException();

                  }, true);
             }).ConfigureAwait(false);
        }

        private async Task AssertTaskAsync(string taskId, Action<BackgroundTaskRunner> startTaskCallback)
        {
            bool taskRan = false;
            Exception lastException = null;
            string lastTaskId = null;
            string lastRunId = null;

            bool taskEnded = false;

            var bgRunner = new BackgroundTaskRunner();
            bgRunner.TaskStarting += (tid, rid, started, reason) =>
            {
                taskRan = true;
                lastTaskId = tid;
                lastRunId = rid;
            };
            bgRunner.TaskEnded += (tid, rid, e) =>
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
