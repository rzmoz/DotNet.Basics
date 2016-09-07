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

        [Test]
        public void AsyncTask_RunSync_IsOnlyRunOnceInvoked()
        {
            var taskRun = false;
            var task = new ManagedTask(rid => { taskRun = true; return Task.CompletedTask; });
            taskRun.Should().BeFalse();
            _taskRunner.Run(task);
            taskRun.Should().BeTrue();
        }

        [Test]
        public async Task AsyncTask_RunAsync_IsOnlyRunOnceInvoked()
        {
            var started = false;
            var task = new ManagedTask(rid => { started = true; return Task.CompletedTask; });
            started.Should().BeFalse();
            await _taskRunner.RunAsync(task).ConfigureAwait(false);
            started.Should().BeTrue();
        }

        [Test]
        public async Task SyncTask_RunAsync_ExceptionIsThrown()
        {
            var started = false;
            var syncTask = new ManagedTask(rid => started = true);
            started.Should().BeFalse();
            await _taskRunner.RunAsync(syncTask).ConfigureAwait(false);
            started.Should().BeTrue();
        }

        [Test]
        public void SyncTask_RunSync_IsOnlyRunOnceInvoked()
        {
            var started = false;
            var syncTask = new ManagedTask(rid => started = true);
            started.Should().BeFalse();
            _taskRunner.Run(syncTask);
            started.Should().BeTrue();
        }
    }
}
