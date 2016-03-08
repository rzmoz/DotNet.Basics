using System.Threading.Tasks;
using DotNet.Basics.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Tasks
{
    [TestFixture]
    public class AsyncTaskTests
    {
        [Test]
        public async Task RunAsync_SyncAction_TaskIsRun()
        {
            var counter = 0;
            var syncTask = new AsyncTask(null, metadata => counter++);

            await syncTask.RunAsync().ConfigureAwait(false);

            counter.Should().Be(1);
        }

        [Test]
        public async Task RunAsync_AsyncAction_TaskIsRun()
        {
            var counter = 0;
            var syncTask = new AsyncTask(null, async metadata => counter++);

            await syncTask.RunAsync().ConfigureAwait(false);

            counter.Should().Be(1);
        }
    }
}
