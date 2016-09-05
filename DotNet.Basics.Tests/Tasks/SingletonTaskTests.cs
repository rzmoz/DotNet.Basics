﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Tasks
{
    [TestFixture]
    public class SingletonTaskTests
    {
        [Test]
        public void ExceptionThrown_GetExceptions_ExceptionsAreBubbled()
        {
            var taskID = "ExceptionThrown_GetExceptions_ExceptionsAreBubbled";

            bool taskRan = false;

            var task = new SingletonTask(() =>
            {
                taskRan = true;
                throw new ArgumentNullException();
            }, taskID);

            Action action = () => task.Run();

            action.ShouldThrow<ArgumentNullException>();
            taskRan.Should().BeTrue();
        }

        [Test]
        public async Task StartTask_DetectTaskIsAlreadyRunning_TaskIsRunOnce()
        {
            string taskId = "StartTask_DetectTaskIsAlreadyRunning_TaskIsRunOnce";

            int hitCount = 0;

            var task = new SingletonTask(async ct =>
            {
                hitCount++;
                await Task.Delay(1.Seconds(), ct).ConfigureAwait(false);
            }, taskId);

            //try start task 10 times
            foreach (var i in Enumerable.Range(1, 10))
                task.RunAsync(CancellationToken.None).ConfigureAwait(false);//don't await task so it runs multiple times

            await WaitTillFinished(task).ConfigureAwait(false);

            hitCount.Should().Be(1);
        }

        [Test]
        public async Task RunAsync_Cancellation_LongRunningTasksCanBeCancelled()
        {
            string taskId = "RunAsync_Cancellation_LongRunningTasksCanBeCancelled";

            bool taskEndedNaturally = false;
            var taskDelay = 5.Seconds();

            var task = new SingletonTask(async (ct) =>
            {
                await Task.Delay(taskDelay, ct).ConfigureAwait(false);
                taskEndedNaturally = true;
            }, taskId);


            bool taskStarted = false;
            var ctSource = new CancellationTokenSource();

            task.Starting += (id, runId, started, reasons) => { taskStarted = started; };

            task.RunAsync(ctSource.Token);//don't await task finish

            while (taskStarted == false)
                await Task.Delay(50.Milliseconds(), ctSource.Token).ConfigureAwait(false);

            task.IsRunning().Should().BeTrue($"task is running");

            //cancel task
            ctSource.Cancel();

            await WaitTillFinished(task).ConfigureAwait(false);
            taskEndedNaturally.Should().BeFalse("task shouldve been cancelled");
            task.IsRunning().Should().BeFalse("task should have been stopped");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public async Task RunAsync_TaskIdEmptyTask_ExceptionIsThrown(string taskId)
        {
            var errorCaught = false;

            try
            {
                var task = new SingletonTask(ct => Task.CompletedTask);
                await task.RunAsync(CancellationToken.None).ConfigureAwait(false);
            }
            catch (ArgumentNullException)
            {
                errorCaught = true;
            }
            catch (ArgumentException)
            {
                errorCaught = true;
            }
            errorCaught.Should().BeTrue("empty task id should fail");
        }

        [Test]
        public async Task RunAsync_Exception_LockIsReleasedOnTaskCrash()
        {
            string taskId = "RunAsync_Exception_LockIsReleasedOnTaskCrash";
            const int runCount = 3;
            var counter = 0;

            var runner = new SingletonTask((ct) =>
            {
                counter++;
                //crash task thread
                throw new ApplicationException("Cowabungaa");
            }, taskId);

            //run task multiple times
            foreach (var i in Enumerable.Range(1, runCount)) //outer sequence awaits til actual running tasks i done
            {
                try
                {
                    await runner.RunAsync(CancellationToken.None).ConfigureAwait(false);
                }
                catch (ApplicationException)
                {
                    //ignore
                }

                await Task.Delay(100.Milliseconds(), CancellationToken.None).ConfigureAwait(false);
            }

            counter.Should().Be(runCount);
        }

        private async Task WaitTillFinished(SingletonTask task)
        {
            while (task.IsRunning())
            {
                Console.WriteLine($"task is running:{task.Id}");
                await Task.Delay(100.Milliseconds()).ConfigureAwait(false);
            }
        }
    }
}
