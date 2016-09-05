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
    public class BackgroundTaskTests
    {
        [Test]
        public async Task RunSync_GetExceptionsFromBgTasks_ExceptionsAreBubbled()
        {
            var taskID = "RunSync_GetExceptionsFromBgTasks_ExceptionsAreBubbled";

            bool taskRan = false;

            var task = new BackgroundTask(() =>
            {
                taskRan = true;
                throw new ArgumentNullException();

            }, taskID);


            Exception lastException = null;
            string lastTaskId = null;
            string lastRunId = null;

            bool taskEnded = false;

            task.Starting += (tid, rid, started, reason) =>
            {
                taskRan = true;
                lastTaskId = tid;
                lastRunId = rid;
            };
            task.Ended += (tid, rid, e) =>
            {
                lastException = e;
                taskEnded = true;
            };

            //start task
            task.Run();

            while (taskEnded == false)
                await Task.Delay(50.Milliseconds()).ConfigureAwait(false);

            taskRan.Should().BeTrue();
            lastTaskId.Should().Be(task.Id);
            lastRunId.Should().NotBeNull();
            lastException.Should().BeOfType<ArgumentNullException>();
        }

        [Test]
        public async Task RunAsync_GetExceptionsFromBgTasks_ExceptionsAreBubbled()
        {
            var taskID = "RunSync_GetExceptionsFromBgTasks_ExceptionsAreBubbled";

            bool taskRan = false;

            var task = new BackgroundTask(() =>
            {
                taskRan = true;
                throw new ArgumentNullException();

            }, taskID);


            Exception lastException = null;
            string lastTaskId = null;
            string lastRunId = null;

            bool taskEnded = false;

            task.Starting += (tid, rid, started, reason) =>
            {
                taskRan = true;
                lastTaskId = tid;
                lastRunId = rid;
            };
            task.Ended += (tid, rid, e) =>
            {
                lastException = e;
                taskEnded = true;
            };

            //start task
            await task.RunAsync(CancellationToken.None).ConfigureAwait(false);

            while (taskEnded == false)
                await Task.Delay(50.Milliseconds()).ConfigureAwait(false);

            taskRan.Should().BeTrue();
            lastTaskId.Should().Be(task.Id);
            lastRunId.Should().NotBeNull();
            lastException.Should().BeOfType<ArgumentNullException>();
        }
    }
}
