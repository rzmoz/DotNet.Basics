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
        private readonly TaskRunner _taskRunner = new TaskRunner();
        
        [Test]
        public void ExceptionThrown_GetExceptions_ExceptionsAreBubbled()
        {
            var taskID = "ExceptionThrown_GetExceptions_ExceptionsAreBubbled";
            bool taskRan = false;
            
            Action action = () => _taskRunner.TryStart(taskID, rid =>
            {
                taskRan = true;
                throw new ArgumentNullException();
            }, RunMode.Singleton);

            action.ShouldThrow<ArgumentNullException>();
            taskRan.Should().BeTrue();
        }

        [Test]
        public async Task StartAsyncTask_DetectTaskIsAlreadyRunning_TaskIsRunOnce()
        {
            string taskId = "StartAsyncTask_DetectTaskIsAlreadyRunning_TaskIsRunOnce";
            int hitCount = 0;
            
            //try start task 10 times
            await Enumerable.Range(1, 10).ParallelForEachAsync(async i =>
                {
                    await _taskRunner.TryStartAsync(taskId, async rid =>
                    {
                        hitCount++;
                        await Task.Delay(1.Seconds()).ConfigureAwait(false);
                    }, RunMode.Singleton).ConfigureAwait(false);
                }).ConfigureAwait(false);

            hitCount.Should().Be(1);
        }
        [Test]
        public void StartSyncTask_DetectTaskIsAlreadyRunning_TaskIsRunOnce()
        {
            string taskId = "StartSyncTask_DetectTaskIsAlreadyRunning_TaskIsRunOnce";
            int hitCount = 0;
            
            //try start task 10 times
            Enumerable.Range(1, 10).ParallelForEach(i =>
            {
                _taskRunner.TryStart(taskId, rid =>
                {
                    hitCount++;
                    Thread.Sleep(1.Seconds());
                }, RunMode.Singleton);
            });

            hitCount.Should().Be(1);
        }

        [Test]
        public async Task RunAsync_Cancellation_LongRunningTasksCanBeCancelled()
        {
            string taskId = "RunAsync_Cancellation_LongRunningTasksCanBeCancelled";

            bool taskRan = false;
            var ctSource = new CancellationTokenSource();
            
            var runTask = _taskRunner.TryStartAsync(taskId, async rid =>
            {
                while (ctSource.Token.IsCancellationRequested == false)
                    await Task.Delay(50.Milliseconds(), CancellationToken.None).ConfigureAwait(false);
                if (ctSource.Token.IsCancellationRequested)
                    return;
                taskRan = true;
            }, RunMode.Singleton);//don't await task finish

            //ensure task has started
            await Task.Delay(100.Milliseconds(), CancellationToken.None).ConfigureAwait(false);

            _taskRunner.IsRunning(taskId).Should().BeTrue($"task is running");

            //cancel task
            ctSource.Cancel();

            await Task.WhenAll(runTask).ConfigureAwait(false);

            taskRan.Should().BeFalse("task shouldve been cancelled");
            _taskRunner.IsRunning(taskId).Should().BeFalse("task should have been stopped");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void Run_TaskIdEmptyTask_ExceptionIsThrown(string taskId)
        {
            var errorCaught = false;
            
            try
            {
                _taskRunner.TryStart(rid => { }, RunMode.Singleton);
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
                    await _taskRunner.TryStartAsync(taskId, async rid =>
                    {
                        counter++;
                        //do a longrunning task
                        await LongRunningTask(taskDelay, 100000, new ApplicationException("Cowabunga"));
                    }, RunMode.Singleton).ConfigureAwait(false);
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
