﻿using System;
using System.Threading.Tasks;
using DotNet.Basics.Tasks.Repeating;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Tasks.Repeating
{
    public class OnceOnlyTaskTests
    {
        [Fact]
        public async Task AsyncTask_Run_ActionIsOnlyExecutedOnce()
        {
            var counter = 0;

#pragma warning disable 1998
            await Run5TimesAsync(async () =>
#pragma warning restore 1998
            {
                counter++;
                return 0;
            });

            counter.Should().Be(1);
        }

        [Fact]
        public async Task AsyncTask_RunWithException_ActionIsOnlyExecutedOnce()
        {
            var counter = 0;

#pragma warning disable 1998
            await Run5TimesAsync(async () =>
#pragma warning restore 1998
            {
                counter++;
                throw new ArgumentException("buuh");
            });

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

        private async Task Run5TimesAsync(Func<Task<int>> task)
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
