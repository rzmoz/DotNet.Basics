using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;
using DotNet.Basics.Tasks.Concurrent;
using DotNet.Basics.Tasks.Repeating;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Tasks.Concurrent
{
    [TestFixture]
    public class InProcConcurrentTaskRunnerTests
    {
        private InProcConcurrentTaskRunner _runner;

        [SetUp]
        public void SetUp()
        {
            _runner = new InProcConcurrentTaskRunner(3.Seconds());
        }

        [Test]
        public async Task RunInBackground_WriteMetadata_MetadaRunInBackground_WriteMetadata_MetadataIsSettaIsSet()
        {
            const string taskName = "RunInBackground_WriteMetadata_MetadataIsSet";
            const string metadataKey = "myKey";
            const string metadataValue = "myValue";

            var startResult = _runner.RunAtMostOnceInBackground(taskName, metadata => metadata.Add(metadataKey, metadataValue));

            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

            var info = _runner.GetTaskInfo(taskName);

            startResult.Should().Be(StartResult.Started);
            info.Metadata[metadataKey].Should().Be(metadataValue);
        }

        [Test]
        public void RunInBackground_RunInBackground_TaskIsExecutedInBackground()
        {
            const string taskName = "RunInBackground_RunInBackground_TaskIsExecutedInBackground";

            const int taskBreakCount = 10;
            var counter = 0;

            var startResult = _runner.RunAtMostOnceInBackground(taskName, async metadata =>
            {
                for (var i = 0; i < taskBreakCount; i++)
                    counter++;
                await Task.Delay(TimeSpan.FromSeconds(1));
            });

            WaitForLockToBeAcquiredAndThenReleased(taskName).Should().BeGreaterOrEqualTo(2);
            startResult.Should().Be(StartResult.Started);
            counter.Should().Be(taskBreakCount);
        }

        [Test]
        public void IsLocked_EnsureTaskIsLockedFromOtherProcesses_LockIsSet()
        {
            const string taskName = "IsLocked_EnsureTaskIsLockedFromOtherProcesses_LockIsSet";

            var counter = 0;

            //run same task in two different threads
            var acquireResult = _runner.RunAtMostOnceInBackground(taskName, async metadata =>
            {
                counter++;
                await Task.Delay(TimeSpan.FromSeconds(3));//to ensure next task has time to start
            });

            var secondLockResult = _runner.RunAtMostOnceInBackground(taskName, metadata => counter++);
            WaitForLockToBeAcquiredAndThenReleased(taskName);

            acquireResult.Should().Be(StartResult.Started);
            secondLockResult.Should().Be(StartResult.AlreadyRunning);
            counter.Should().Be(1);//only one of them should run
        }

        [Test]
        public async Task IsLocked_LockIsRenewed_TaskIsLocked()
        {
            const string taskName = "IsLocked_LockIsRenewed_TaskIsLocked";

            var lockTimeout = TimeSpan.FromSeconds(1);

            var expectedCount = 5;
            //run same task in two different threads
            var runner = new InProcConcurrentTaskRunner(lockTimeout);

            var startTime = DateTime.UtcNow;

            var acquireResult = runner.RunAtMostOnceInBackground(taskName, async metadata =>
            {
                //running an operation that's longer than default timeout
                for (var i = 0; i < expectedCount; i++)
                {
                    metadata.Add("key" + i, i.ToString());
                    await Task.Delay(lockTimeout);
                }
            });
            await Task.Delay(lockTimeout);//wait one lock timeout to ensure the lock has been acquired

            var isLocked = _runner.IsLocked(taskName);
            acquireResult.Should().Be(StartResult.Started);

            isLocked.Should().BeTrue();
            while (_runner.IsLocked(taskName)) { await Task.Delay(TimeSpan.FromMilliseconds(200)); }

            var stopTime = DateTime.UtcNow;
            var waitTime = stopTime - startTime;

            waitTime.Should().BeGreaterOrEqualTo(TimeSpan.FromTicks(lockTimeout.Ticks * expectedCount));
        }

        private int WaitForLockToBeAcquiredAndThenReleased(string taskName)
        {
            var waitCounter = 0;
            //wait til lock is acquired
            Repeat.Task(() => { Trace.TraceInformation("Wait for Acquire lock: {0}", taskName); }).
                WithTimeout(TimeSpan.FromSeconds(5))
                .Until(() => _runner.IsLocked(taskName))
                .Now();
            //wait till lock is released
            Repeat.Task(() => waitCounter++)
                .WithPing(() => Trace.TraceInformation("Wait for Release lock: {0}", taskName))
                .WithTimeout(TimeSpan.FromMinutes(1))
                .Until(() => _runner.IsLocked(taskName) == false)
                .Now();

            return waitCounter;
        }
    }
}
