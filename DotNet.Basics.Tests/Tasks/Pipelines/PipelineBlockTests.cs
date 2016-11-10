using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;
using DotNet.Basics.Tasks.Pipelines;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Tasks.Pipelines
{
    public class PipelineBlockTests
    {
        [Fact]
        public void Add_AddGenericSteps_StepsAreAdded()
        {
            var block = new Pipeline();
            var taskCount = 7;

            foreach (var i in Enumerable.Range(0, taskCount))
                block.AddStep<ManagedTask<EventArgs>>();

            block.Tasks.Count().Should().Be(taskCount);
        }

        [Fact]
        public void Add_AddInlineTasks_TasksAreAdded()
        {
            var block = new Pipeline<EventArgs<int>>();
            var taskCount = 7;

            foreach (var i in Enumerable.Range(0, taskCount))
                block.AddStep((args, issues, ct) => { });

            block.Tasks.Count().Should().Be(taskCount);
        }

        [Theory]
        [InlineData(101, Invoke.Parallel)]
        [InlineData(10, Invoke.Sequential)]
        public async Task BlockInvokeStyle_Sequence_TasksAreRunAccordingToInvokeStyle(int stepCount, Invoke invoke)
        {
            var block = new Pipeline<EventArgs<int>>(invoke);
            int lockFlag = 0;

            var raceConditionEncountered = 0;
            for (var i = 0; i < stepCount; i++)
                block.AddStep(async (args, issues, ct) =>
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

            if (invoke == Invoke.Parallel)
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
