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
        private readonly ManagedTaskFactory _taskFactory = new ManagedTaskFactory();

        [Test]
        public void Run_SyncTask_TaskIsRun()
        {
            var taskRan = false;

            var task = _taskFactory.Create<ManagedTask>(rid => taskRan = true);

            taskRan.Should().BeFalse();
            _taskRunner.TryStart(task);
            taskRan.Should().BeTrue();
        }
        [Test]
        public void Run_AsyncTask_TaskIsRun()
        {
            var taskRan = false;

            var task = _taskFactory.Create<ManagedTask>(async rid =>
        {
            await VoidTaskAsync();//ensure async execution
                taskRan = true;
        });

            taskRan.Should().BeFalse();
            _taskRunner.TryStart(task);
            taskRan.Should().BeTrue();
        }
        [Test]
        public async Task RunAsync_SyncTask_TaskIsRun()
        {
            var taskRan = false;

            var task = _taskFactory.Create<ManagedTask>(rid => taskRan = true);

            taskRan.Should().BeFalse();
            await _taskRunner.TryStartAsync(task).ConfigureAwait(false);
            taskRan.Should().BeTrue();
        }
        [Test]
        public async Task RunAsync_AsyncTask_TaskIsRun()
        {
            var taskRan = false;

            var task = _taskFactory.Create<ManagedTask>(async rid =>
            {
                await VoidTaskAsync();//ensure async execution
                taskRan = true;
            });

            taskRan.Should().BeFalse();
            await _taskRunner.TryStartAsync(task).ConfigureAwait(false);
            taskRan.Should().BeTrue();
        }

        private Task VoidTaskAsync()
        {
            return Task.CompletedTask;
        }
    }
}
