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
    public class AtMostOnceTaskRunnerTests
    {
        [Test]
        public async Task ExceptionThrown_GetExceptionsFromBgTasks_ExceptionsAreBubbled()
        {
            string taskId = "ExceptionThrown_GetExceptionsFromBgTasks_ExceptionsAreBubbled";
            Exception exceptionCaughtFromEvent = null;
            string taskIdCaughtFromEvent = null;

            Exception exceptionCaughtFromCallBack = null;
            string taskIdCaughtFromCallBack = null;

            var runner = new AtMostOnceTaskRunner();
            runner.TaskFailed += (id, e) =>
             {
                 taskIdCaughtFromEvent = id;
                 exceptionCaughtFromEvent = e;
             };
            runner.StartTask(taskId, ct =>
            {
                throw new ArgumentNullException();
            }, (id, e) =>
            {
                taskIdCaughtFromCallBack = id;
                exceptionCaughtFromCallBack = e;
            });

            await WaitTillFinished(runner, taskId).ConfigureAwait(false);
            taskIdCaughtFromEvent.Should().Be(taskId);
            exceptionCaughtFromEvent.Should().BeOfType<ArgumentNullException>();
            taskIdCaughtFromCallBack.Should().Be(taskId);
            exceptionCaughtFromCallBack.Should().BeOfType<ArgumentNullException>();
        }


        [Test]
        public async Task StartTask_DetectTaskIsAlreadyunning_TaskIsRunOnce()
        {
            var runner = new AtMostOnceTaskRunner();
            string taskId = "StartTask_TaskRun_TaskIsRunOnce";

            int hitCount = 0;

            Func<CancellationToken, Task> incrementTask = async (ct) =>
            {
                hitCount++;
                await Task.Delay(1.Seconds(), ct).ConfigureAwait(false);
            };

            //try start task 10 times
            foreach (var i in Enumerable.Range(1, 10))
                runner.StartTask(taskId, incrementTask);

            await WaitTillFinished(runner, taskId).ConfigureAwait(false);

            hitCount.Should().Be(1);
        }


        [Test]
        public async Task RunAsync_Cancellation_LongRunningTasksCanBeCancelled()
        {
            var taskDelay = 5.Seconds();
            bool taskEndedNaturally = false;
            Func<CancellationToken, Task> neverEndingTask = async (ct) =>
            {
                await Task.Delay(taskDelay, ct).ConfigureAwait(false);
                taskEndedNaturally = true;
            };

            var ctSource = new CancellationTokenSource();
            string taskId = "RunAsync_IsRunning_TaskProgressIsDetected";

            var runner = new AtMostOnceTaskRunner();

            var runResult = runner.StartTask(taskId, neverEndingTask, ctSource.Token);
            runResult.Started.Should().BeTrue("task started");

            runner.IsRunning(taskId).Should().BeTrue($"task is running");

            //cancel task
            ctSource.Cancel();

            await WaitTillFinished(runner, taskId).ConfigureAwait(false);
            taskEndedNaturally.Should().BeFalse("task shouldve been cancelled");
            runner.IsRunning(taskId).Should().BeFalse("task should have been stopped");
        }


        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void RunAsync_TaskIdEmptyTask_ExceptionIsThrown(string taskId)
        {
            var errorCaught = false;

            try
            {
                var runner = new AtMostOnceTaskRunner();
                runner.StartTask(taskId, (ct) => Task.CompletedTask);
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
        public async Task RunAsync_Exception__LockIsReleasedOnTaskCrash()
        {
            string taskId = "RunAsync_Exception__LockIsReleasedOnTaskCrash";
            const int runCount = 3;

            var counter = 0;

            var runner = new AtMostOnceTaskRunner();

            //run task multiple times
            foreach (var i in Enumerable.Range(1, runCount)) //outer sequence awaits til actual running tasks i done
            {
                try
                {
                    runner.StartTask(taskId, (ct) =>
                    {
                        counter++;
                        //crash task thread
                        throw new ApplicationException("Cowabungaa");
                    });
                }
                catch (ApplicationException)
                {
                    //ignore
                }

                await Task.Delay(100.Milliseconds(), CancellationToken.None).ConfigureAwait(false);
            }

            counter.Should().Be(runCount);
        }

        private async Task WaitTillFinished(AtMostOnceTaskRunner runner, string taskId)
        {
            while (runner.IsRunning(taskId))
            {
                System.Console.WriteLine($"task is running:{taskId}");
                await Task.Delay(100.Milliseconds()).ConfigureAwait(false);
            }
        }
    }
}
