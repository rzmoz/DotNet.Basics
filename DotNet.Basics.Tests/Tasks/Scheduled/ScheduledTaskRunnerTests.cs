using System.Threading.Tasks;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;
using DotNet.Basics.Tasks.Scheduling;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Tasks.Scheduled
{
    [TestFixture]
    public class ScheduledTaskRunnerTests
    {
        [Test]
        public async Task StartStop_RunScheduled_TasksIsRunForDuration()
        {
            int counter = 0;

            var task = new SyncTask(null, metadata => counter++);
            using (new ScheduledTaskRunner(task, 100.MilliSeconds())) await Task.Delay(TimeSpanExtensions.Seconds(1)).ConfigureAwait(false);

            counter.Should().BeInRange(8, 12);//should happen 9-11 times
        }

        [Test]
        public async Task Run_RunScheduled_TasksIsRun()
        {
            int counter = 0;

            var task = new SyncTask(null, metadata => counter++);
            var trigger = new ScheduledTaskRunner(task, 100.MilliSeconds(), StartOptions.InitStopped);
            await Task.Delay(TimeSpanExtensions.Seconds(1)).ConfigureAwait(false);
            counter.Should().Be(0);//not started yet

            trigger.Start();
            await Task.Delay(TimeSpanExtensions.Seconds(1)).ConfigureAwait(false); //run task
            trigger.Stop();

            await Task.Delay(TimeSpanExtensions.Seconds(1)).ConfigureAwait(false);//ensure task is stopped
            counter.Should().BeInRange(8, 12);//should happen 9-11 times
        }

        [Test]
        public async Task Dispose_StopTimer_TimerIsKilled()
        {
            int counter = 0;

            var task = new SyncTask(null, metadata => counter++);
            using (new ScheduledTaskRunner(task, 100.MilliSeconds())) await Task.Delay(TimeSpanExtensions.Seconds(1)).ConfigureAwait(false);

            await Task.Delay(TimeSpanExtensions.Seconds(1)).ConfigureAwait(false);//wait further but timer should be garbage collected already
            counter.Should().BeInRange(8, 12);//should happen 9-11 times
        }


        [Test]
        public async Task Run_InheritTrigger_DerivedRunIsTriggered()
        {
            int counter = 0;

            var task = new SyncTask(null, metadata => counter++);
            using (new ScheduledTaskRunner(task, 100.MilliSeconds()))
            using (new VoidTaskRunnerTests(task, 10.MilliSeconds()))
                await Task.Delay(TimeSpanExtensions.Seconds(1)).ConfigureAwait(false);

            counter.Should().BeInRange(8, 12);//should happen 9-11 times since only first trigger is fired
        }
    }
}

