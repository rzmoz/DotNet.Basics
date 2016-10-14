using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Tasks
{
    [TestFixture]
    public class ManagedTaskTests
    {
        [Test]
        public async Task RunAsync_SyncTask_TaskIsOnlyRunWhenInvoked()
        {
            var taskRan = false;

            var task = new ManagedTask<EventArgs>(() => taskRan = true);

            taskRan.Should().BeFalse();
            await task.RunAsync().ConfigureAwait(false);
            taskRan.Should().BeTrue();
        }
        [Test]
        public async Task RunAsync_AsyncTask_TaskIsOnlyRunWhenInvoked()
        {
            var taskRan = false;

            var task = new ManagedTask<EventArgs>(async () =>
                {
                    await VoidTaskAsync().ConfigureAwait(false);//ensure async execution
                    taskRan = true;
                }
            );

            taskRan.Should().BeFalse();
            await task.RunAsync().ConfigureAwait(false);
            taskRan.Should().BeTrue();
        }

        private Task VoidTaskAsync()
        {
            return Task.CompletedTask;
        }
    }
}
