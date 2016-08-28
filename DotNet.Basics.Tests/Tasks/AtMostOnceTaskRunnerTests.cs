using System;
using System.Collections.Generic;
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
        public async Task RunAsync_IsRunning_TaskProgressIsDetected()
        {
            var ctSource = new CancellationTokenSource();
            string id = "RunAsync_IsRunning_TaskProgressIsDetected";
            var taskTimeOut = 100.Milliseconds();

            var runner = new AtMostOnceTaskRunner();
            var runResult = await runner.RunAsync(id, async (ct) =>
            {
                while (true)
                    // ReSharper disable once MethodSupportsCancellation
                    await Task.Delay(taskTimeOut).ConfigureAwait(false);
            }, ctSource.Token, taskTimeOut).ConfigureAwait(false);

            runResult.Started.Should().BeTrue("task started");
            await Task.Delay(taskTimeOut + taskTimeOut + taskTimeOut).ConfigureAwait(false);//wait a couple of timeouts to ensure lock is renewing
            runner.IsRunning(id).Should().BeTrue("task is running");
            await Task.Delay(taskTimeOut.Add(50.Milliseconds())).ConfigureAwait(false);//wait till we're sure the lock has been released
            runner.IsRunning(id).Should().BeFalse("task should been stopped");
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
                await runner.RunAsync(taskId, async (ct) => { });
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
        public async Task RunAsync_DetectRunningTask_TaskIsOnlyRunOnce()
        {
            var counter = 0;
            Func<CancellationToken, Task> task = async (ct) =>
            {
                counter++;
                await Task.Delay(500.Milliseconds()).ConfigureAwait(false);
            };

            var runner = new AtMostOnceTaskRunner();

            var tasks = new List<Task>();

            //run task many times
            foreach (var i in Enumerable.Range(1, 20))
                tasks.Add(runner.RunAsync("myId", task));

            await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);
            counter.Should().Be(1);
        }

        [Test]
        public async Task RunAsync_TaskLockRelease_LockIsReleasedOnceDone()
        {
            const int runCount = 5;

            var counter = 0;
            Func<CancellationToken, Task> task = async (ct) =>
            {
                counter++;
                await Task.Delay(100.Milliseconds()).ConfigureAwait(false);
            };

            var runner = new AtMostOnceTaskRunner();

            //run task many times
            foreach (var i in Enumerable.Range(1, runCount)) //outer sequence awaits til actual running tasks i done
            {
                var tasks = new List<Task>();
                foreach (var j in Enumerable.Range(1, 10)) //add filler tasks to show they're still discarded
                    tasks.Add(runner.RunAsync("myId", task));
                await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);
            }

            counter.Should().Be(runCount);
        }

        [Test]
        public async Task RunAsync_Exception__LockIsReleasedOnTaskCrash()
        {
            const int runCount = 3;

            var counter = 0;
            Func<CancellationToken, Task> task = async (ct) =>
               {
                   counter++;
                   //crash task thread
                   throw new ApplicationException("Cowabungaa");
               };

            var runner = new AtMostOnceTaskRunner();

            //run task many times
            foreach (var i in Enumerable.Range(1, runCount)) //outer sequence awaits til actual running tasks i done
            {
                try
                {
                    await runner.RunAsync("myId", task).ConfigureAwait(false);
                }
                catch (ApplicationException)
                {
                    //ignore
                }
            }

            counter.Should().Be(runCount);
        }

        [Test]
        public async Task RunAsync_LongrunningTask_LockIsRenewedWhenTaskRunsLong()
        {
            var timeout = 200.Milliseconds();

            var counter = 0;
            Func<CancellationToken, Task> task = async (ct) =>
            {
                counter++;
                await Task.Delay(timeout + timeout + timeout); //task duration is 3 times longer than timeout
            };

            var runner = new AtMostOnceTaskRunner();
            var tasks = new List<Task>();

            //run task many times
            foreach (var i in Enumerable.Range(1, 10))
                tasks.Add(runner.RunAsync("myId", task));

            await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);

            counter.Should().Be(1);
        }
    }
}
