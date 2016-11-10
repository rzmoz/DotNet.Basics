using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Sys;
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
            var stepBlock = new PipelineBlock<EventArgs<int>>(null, null)
                .AddStep<IncrementArgsStep>()
                .AddStep<IncrementArgsStep>()
                .AddStep<IncrementArgsStep>()
                .AddStep<IncrementArgsStep>()
                .AddStep<IncrementArgsStep>();

            stepBlock.Count().Should().Be(5);
        }

        [Fact]
        public async Task BlockRunType_Parallel_StepsAreRunInParallel()
        {
            var stepCount = 101;
            await RunBlockAndCheckConditionsAsync(stepCount, BlockRunType.Parallel);
        }

        [Fact]
        public async Task BlockRunType_Sequence_StepsAreRunInSequence()
        {
            var stepCount = 10;
            await RunBlockAndCheckConditionsAsync(stepCount, BlockRunType.Sequential);
        }

        private async Task RunBlockAndCheckConditionsAsync(int stepCount, BlockRunType blockRunType)
        {
            var block = new PipelineBlock<EventArgs<int>>(blockRunType);
            int lockFlag = 0;

            var raceConditionEncountered = 0;
            for (var i = 0; i < stepCount; i++)
                block.AddStep(async (args, ct) =>
                {
                    if (Interlocked.CompareExchange(ref lockFlag, 1, 0) == 0)
                    {
                        Monitor.Enter(lockFlag);
                        args.Value = args.Value + 1;
                        await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false);
                        // free the lock.
                        Interlocked.Decrement(ref lockFlag);
                    }
                    else
                        raceConditionEncountered++;
                });

            var argsOutput = await block.RunAsync(CancellationToken.None).ConfigureAwait(false);
            if (blockRunType == BlockRunType.Parallel)
            {
                argsOutput.Value.Should().BeLessThan(stepCount);
                raceConditionEncountered.Should().BeGreaterThan(0);
            }
            else
            {
                argsOutput.Value.Should().Be(stepCount);
                raceConditionEncountered.Should().Be(0);
            }
        }
    }
}
