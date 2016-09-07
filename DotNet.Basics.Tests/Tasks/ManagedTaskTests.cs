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
        private readonly TaskRunner _taskRunner = new TaskRunner();

        [Test]
        public void Run_SyncTask_TaskIsRun()
        {
            var taskRan = false;

            var task = new ManagedTask(rid => taskRan = true);

            taskRan.Should().BeFalse();
            _taskRunner.Run(task);
            taskRan.Should().BeTrue();
        }
        [Test]
        public void Run_AsyncTask_TaskIsRun()
        {
            var taskRan = false;

            var task = new ManagedTask(async rid =>
            {
                await VoidTaskAsync();//ensure async execution
                taskRan = true;
            });

            taskRan.Should().BeFalse();
            _taskRunner.Run(task);
            taskRan.Should().BeTrue();
        }
        [Test]
        public async Task RunAsync_SyncTask_TaskIsRun()
        {
            var taskRan = false;

            var task = new ManagedTask(rid => taskRan = true);

            taskRan.Should().BeFalse();
            await _taskRunner.RunAsync(task).ConfigureAwait(false);
            taskRan.Should().BeTrue();
        }
        [Test]
        public async Task RunAsync_AsyncTask_TaskIsRun()
        {
            var taskRan = false;

            var task = new ManagedTask(async rid =>
            {
                await VoidTaskAsync();//ensure async execution
                taskRan = true;
            });

            taskRan.Should().BeFalse();
            await _taskRunner.RunAsync(task).ConfigureAwait(false);
            taskRan.Should().BeTrue();
        }

        private Task VoidTaskAsync()
        {
            return Task.CompletedTask;
        }
    }
}
