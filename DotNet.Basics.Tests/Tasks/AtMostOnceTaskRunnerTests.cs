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
    public class AtMostOnceTaskRunnerTests
    {
        private AtMostOnceTaskRunner _runner;

        [SetUp]
        public void Setup()
        {
            _runner = new AtMostOnceTaskRunner();
        }

        private async Task WaitTillFinished(AtMostOnceTaskRunner runner, string taskId)
        {
            while (runner.IsRunning(taskId))
            {
                System.Console.WriteLine($"task is running:{taskId}");
                await Task.Delay(100.Milliseconds()).ConfigureAwait(false);
            }
        }

        [Test]
        public async Task StartTask_DetectTaskIsAlreadyunning_TaskIsRunOnce()
        {
            string taskId = "StartTask_TaskRun_TaskIsRunOnce";

            int hitCount = 0;

            Func<CancellationToken, Task> incrementTask = async (ct) =>
            {
                hitCount++;
                await Task.Delay(1.Seconds(), ct).ConfigureAwait(false);
            };

            //try start task 10 times
            foreach (var i in Enumerable.Range(1, 10))
                _runner.StartTask(taskId, incrementTask);

            await WaitTillFinished(_runner, taskId).ConfigureAwait(false);

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

            await WaitTillFinished(_runner, taskId).ConfigureAwait(false);
            taskEndedNaturally.Should().BeFalse("task shouldve been cancelled");
            runner.IsRunning(taskId).Should().BeFalse("task should have been stopped");
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
                var runner = new AtMostOnceTaskRunner();
                runner.StartTask(taskId, async (ct) => { });
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
            Func<CancellationToken, Task> task = async (ct) =>
               {
                   counter++;
                   //crash task thread
                   throw new ApplicationException("Cowabungaa");
               };

            var runner = new AtMostOnceTaskRunner();

            //run task multiple times
            foreach (var i in Enumerable.Range(1, runCount)) //outer sequence awaits til actual running tasks i done
            {
                try
                {
                    runner.StartTask(taskId, task);
                }
                catch (ApplicationException)
                {
                    //ignore
                }
            }

            counter.Should().Be(runCount);
        }
    }
}
