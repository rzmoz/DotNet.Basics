using System;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Tasks
{
    
    public class OnceOnlyTaskTests
    {
        [Fact]
        public async Task AsyncTask_Run_ActionIsOnlyExecutedOnce()
        {
            var counter = 0;

            await Run5TimesAsync(async () =>
            {
                counter++;
            }).ConfigureAwait(false);

            counter.Should().Be(1);
        }

        [Fact]
        public async Task AsyncTask_RunWithException_ActionIsOnlyExecutedOnce()
        {
            var counter = 0;

            await Run5TimesAsync(async () =>
            {
                counter++;
                throw new ArgumentException("buuh");
            }).ConfigureAwait(false);

            counter.Should().Be(1);
        }

        [Fact]
        public void SyncTask_Run_ActionIsOnlyExecutedOnce()
        {
            var counter = 0;

            Run5Times(() =>
            {
                counter++;
            });
            counter.Should().Be(1);
        }

        [Fact]
        public void SyncTask_RunWithException_ActionIsOnlyExecutedOnce()
        {
            var counter = 0;

            Run5Times(() =>
            {
                counter++;
                throw new ArgumentException("buuh");
            });
            counter.Should().Be(1);
        }


        private async Task Run5TimesAsync(Func<Task> task)
        {
            var ooTask = task.ToOnceOnly();

            //invoke multiple times
            for (var i = 0; i < 5; i++)
                try
                {
                    await ooTask.Invoke().ConfigureAwait(false);
                }
                catch (ArgumentException e)
                {
                    e.Message.Should().Be("buuh");
                }
        }

        private void Run5Times(Action task)
        {
            var ooTask = task.ToOnceOnly();

            //invoke multiple times
            for (var i = 0; i < 5; i++)
                try
                {
                    ooTask.Invoke();
                }
                catch (ArgumentException e)
                {
                    e.Message.Should().Be("buuh");
                }
        }
    }
}
