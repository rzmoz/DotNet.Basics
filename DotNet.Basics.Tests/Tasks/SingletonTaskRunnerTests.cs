using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Collections;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Tasks
{
    [TestFixture]
    public class SingletonTaskRunnerTests
    {
        private readonly SingletonTaskRunner _taskRunner = new SingletonTaskRunner();

        [Test]
        public void ExceptionThrown_GetExceptions_ExceptionsAreBubbled()
        {
            var taskID = "ExceptionThrown_GetExceptions_ExceptionsAreBubbled";

            bool taskRan = false;

            Action action = () => _taskRunner.Run(taskID, rid =>
            {
                taskRan = true;
                throw new ArgumentNullException();
            });

            action.ShouldThrow<ArgumentNullException>();
            taskRan.Should().BeTrue();
        }

        [Test]
        public async Task StartTask_DetectTaskIsAlreadyRunning_TaskIsRunOnce()
        {
            string taskId = "StartTask_DetectTaskIsAlreadyRunning_TaskIsRunOnce";

            int hitCount = 0;

            //try start task 10 times
            await Enumerable.Range(1, 10).ParallelForEachAsync(async i =>
                {
                    await _taskRunner.RunAsync(taskId, async rid =>
                    {
                        hitCount++;
                        await Task.Delay(1.Seconds()).ConfigureAwait(false);
                    }).ConfigureAwait(false);
                }).ConfigureAwait(false);

            hitCount.Should().Be(1);
        }

        [Test]
        public async Task RunAsync_Cancellation_LongRunningTasksCanBeCancelled()
        {
            string taskId = "RunAsync_Cancellation_LongRunningTasksCanBeCancelled";

            bool taskRan = false;
            var taskDelay = 5.Seconds();

            var ctSource = new CancellationTokenSource();

            _taskRunner.RunAsync(taskId, async rid =>
            {
                await Task.Delay(taskDelay, ctSource.Token).ConfigureAwait(false);
                taskRan = true;
            });//don't await task finish

            _taskRunner.IsRunning(taskId).Should().BeTrue($"task is running");

            //cancel task
            ctSource.Cancel();

            while (_taskRunner.IsRunning(taskId))
                await Task.Delay(100.Milliseconds(), CancellationToken.None).ConfigureAwait(false);

            taskRan.Should().BeFalse("task shouldve been cancelled");
            _taskRunner.IsRunning(taskId).Should().BeFalse("task should have been stopped");
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
                await _taskRunner.RunAsync(taskId, rid => Task.CompletedTask).ConfigureAwait(false);
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
            const int runCount = 5;
            var counter = 0;
            var taskDelay = 200.Milliseconds();

            var expectedDuration = ((int)(taskDelay.TotalMilliseconds * runCount)).Milliseconds();

            var profiler = new Profiler();
            profiler.Start();
            //run task multiple times in sequence
            foreach (var i in Enumerable.Range(1, runCount))
            {
                try
                {
                    await _taskRunner.RunAsync(taskId, async rid =>
                     {
                         counter++;
                         //do a longrunning task
                         await LongRunningTask(taskDelay, 100000, new ApplicationException("Cowabunga"));
                     }).ConfigureAwait(false);
                }
                catch (ApplicationException)
                {
                    //ignore
                }
            }
            profiler.Stop();

            profiler.Duration.Should().BeCloseTo(expectedDuration, 50);
            counter.Should().Be(runCount);
        }

        private async Task LongRunningTask(TimeSpan delay, int loops, Exception eToBeThrown)
        {
            for (var i = 0; i < loops; i++)
            {
                await Task.Delay(delay).ConfigureAwait(false);
                if (eToBeThrown != null)
                    throw eToBeThrown;
            }
        }
    }
}
