using System;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Tasks
{
    [TestFixture]
    public class OnlyRunWhenInvokedTaskTests
    {
        [Test]
        public void AsyncTask_RunSync_IsOnlyRunOnceInvoked()
        {
            var voidVar = 0;
            var task = new AsyncTask((ct) => { voidVar++; return Task.CompletedTask; });
            Action action = () => task.Run();
            action.ShouldThrow<NotSupportedException>();
        }

        [Test]
        public async Task AsyncTask_RunAsync_IsOnlyRunOnceInvoked()
        {
            var started = false;
            var task = new AsyncTask((ct) => { started = true; return Task.CompletedTask; });
            started.Should().BeFalse();
            await task.RunAsync().ConfigureAwait(false);
            started.Should().BeTrue();
        }

        [Test]
        public async Task SyncTask_RunAsync_ExceptionIsThrown()
        {
            var started = false;
            var syncTask = new SyncTask(() => started = true);
            started.Should().BeFalse();
            await syncTask.RunAsync();
            started.Should().BeTrue();
        }

        [Test]
        public void SyncTask_RunSync_IsOnlyRunOnceInvoked()
        {
            var started = false;
            var syncTask = new SyncTask(() => started = true);
            started.Should().BeFalse();
            syncTask.Run();
            started.Should().BeTrue();
        }
    }
}
