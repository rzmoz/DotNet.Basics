using DotNet.Basics.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Tasks
{
    [TestFixture]
    public class SyncTaskTests
    {
        [Test]
        public void Run_RunTask_TaskIsRun()
        {
            var counter = 0;
            var syncTask = new SyncTask(null, metadata => counter++);

            syncTask.Run();

            counter.Should().Be(1);
        }
    }
}
