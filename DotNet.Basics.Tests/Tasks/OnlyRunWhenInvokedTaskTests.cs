using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Tasks
{
    [TestFixture]
    public class OnlyRunWhenInvokedTaskTests
    {
        private readonly TaskRunner _taskRunner = new TaskRunner();
        private readonly ManagedTaskFactory _taskFactory = new ManagedTaskFactory();

        [Test]
        public void AsyncTask_RunSync_OnlyRunWhenInvoked()
        {
            var taskRun = false;
            var task = _taskFactory.Create<ManagedTask>(rid => { taskRun = true; return Task.CompletedTask; });
            taskRun.Should().BeFalse();
            _taskRunner.TryStart(task);
            taskRun.Should().BeTrue();
        }

        [Test]
        public async Task AsyncTask_RunAsync_OnlyRunWhenInvoked()
        {
            var started = false;
            var task = _taskFactory.Create<ManagedTask>(rid => { started = true; return Task.CompletedTask; });
            started.Should().BeFalse();
            await _taskRunner.TryStartAsync(task).ConfigureAwait(false);
            started.Should().BeTrue();
        }

        [Test]
        public async Task SyncTask_RunAsync_ExceptionIsThrown()
        {
            var started = false;
            var syncTask = _taskFactory.Create<ManagedTask>(rid => started = true);
            started.Should().BeFalse();
            await _taskRunner.TryStartAsync(syncTask).ConfigureAwait(false);
            started.Should().BeTrue();
        }

        [Test]
        public void SyncTask_RunSync_OnlyRunWhenInvoked()
        {
            var started = false;
            var syncTask = _taskFactory.Create<ManagedTask>(rid => started = true);
            started.Should().BeFalse();
            _taskRunner.TryStart(syncTask);
            started.Should().BeTrue();
        }
    }
}
