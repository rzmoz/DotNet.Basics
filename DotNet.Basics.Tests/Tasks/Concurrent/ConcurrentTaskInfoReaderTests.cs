using System;
using System.Threading.Tasks;
using DotNet.Basics.Tasks.Concurrent;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Tasks.Concurrent
{
    [TestFixture]
    public class ConcurrentTaskInfoReaderTests
    {
        private IConcurrentTaskRunner _runner;
        private IConcurrentTaskInfoReader _infoReader;

        [SetUp]
        public void SetUp()
        {
            _runner = new InProcConcurrentTaskRunner();
            _infoReader = _runner;
        }

        [Test]
        public async Task GetStatus_Status_Processing()
        {
            const string taskName = "GetStatus_Status_Processing";
            _runner.ForceReleaseLock(taskName);

            //used for ensuring we read the result when the task is actually processing without relying on delays
            bool taskStarted = false;
            bool statusGotten = false;
            bool taskEnded = false;

            _runner.RunAtMostOnceInBackground(taskName, async metadata =>
            {
                taskStarted = true;
                while (!statusGotten) { await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false); }
                taskEnded = true;
            });

            while (!taskStarted) { await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false); }

            var status = _infoReader.GetStatus(taskName);
            statusGotten = true;

            while (!taskEnded) { await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false); }

            status.TaskState.Should().Be(TaskState.Processing);
        }

        [Test]
        public void GetStatus_Status_NotFound()
        {
            const string taskName = "GetStatus_Status_NotFound";
            _runner.ForceReleaseLock(taskName);

            var status = _runner.GetStatus(taskName);

            status.TaskState.Should().Be(TaskState.NotFound);
        }

        [Test]
        public async Task GetStatus_Status_Done()
        {
            const string taskName = "GetStatus_Status_Done";
            _runner.EraseIfNotRunning(taskName);
            //used for ensuring we read the result when the task is actually processing without relying on delays
            bool taskEnded = false;

            _runner.RunAtMostOnceInBackground(taskName, metadata => taskEnded = true);

            while (!taskEnded) { await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false); }
            var status = _infoReader.GetStatus(taskName);
            status.TaskState.Should().Be(TaskState.Done);
        }
        [Test]
        public async Task GetStatus_Status_Error()
        {
            const string taskName = "GetStatus_Status_Error";
            _runner.ForceReleaseLock(taskName);

            //used for ensuring we read the result when the task is actually processing without relying on delays
            bool taskCrashed = false;

            var locktimeout = TimeSpan.FromSeconds(2);
            var runner = new InProcConcurrentTaskRunner(locktimeout);
            runner.RunAtMostOnceInBackground(taskName, metadata =>
            {
                try
                {
                    throw new ApplicationException();
                }
                finally
                {
                    taskCrashed = true;
                }
            });

            while (!taskCrashed) { await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false); }
            while (runner.IsLocked(taskName)) { await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false); }

            var status = _infoReader.GetStatus(taskName);
            status.TaskState.Should().Be(TaskState.Error);
        }
    }
}
