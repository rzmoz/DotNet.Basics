﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Tasks
{
    public class ManagedTaskBlockTests
    {
        [Fact]
        public void Add_AddInlineTasks_TasksAreAdded()
        {
            var block = new ManagedTaskBlock<EventArgs<int>>();
            var taskCount = 7;

            foreach (var i in Enumerable.Range(0, taskCount))
                block.AddTask((args, issues, ct) => { });

            block.Tasks.Count().Should().Be(taskCount);
        }

        [Theory]
        [InlineData(101, InvokeStyle.Parallel)]
        [InlineData(10, InvokeStyle.Sequential)]
        public async Task BlockInvokeStyle_Sequence_TasksAreRunAccordingToInvokeStyle(int stepCount, InvokeStyle invokeStyle)
        {
            var block = new ManagedTaskBlock<EventArgs<int>>(invokeStyle);
            int lockFlag = 0;

            var raceConditionEncountered = 0;
            for (var i = 0; i < stepCount; i++)
                block.AddTask(async (args, issues, ct) =>
                {
                    issues.Add(i.ToString());

                    if (Interlocked.CompareExchange(ref lockFlag, 1, 0) == 0)
                    {
                        Monitor.Enter(lockFlag);
                        args.Value = args.Value + 1;
                        await Task.Delay(TimeSpan.FromMilliseconds(100), CancellationToken.None).ConfigureAwait(false);
                        // free the lock.
                        Interlocked.Decrement(ref lockFlag);
                    }
                    else
                        raceConditionEncountered++;
                });

            var result = await block.RunAsync(CancellationToken.None).ConfigureAwait(false);

            result.Issues.Count.Should().Be(stepCount);

            if (invokeStyle == InvokeStyle.Parallel)
            {
                result.Args.Value.Should().BeLessThan(stepCount);
                raceConditionEncountered.Should().BeGreaterThan(0);
            }
            else
            {
                result.Args.Value.Should().Be(stepCount);
                raceConditionEncountered.Should().Be(0);
            }
        }
    }
}
