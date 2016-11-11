using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Collections;
using DotNet.Basics.Diagnostics;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace DotNet.Basics.Tests.Collections
{
    public class CollectionExtensionsTests
    {
        [Fact]
        public async Task ParallelForEachAsync_ParallelExecution_AllTasksAreInvokedAndAwaited()
        {
            var ones = Enumerable.Repeat(1, 101).ToArray();
            var results = new List<int>();

            var singleTaskDuration = 1.Seconds();//keep small to avoid long running tests but also, make it big enough to ensure tasks are run in parallel

            var profiler = new Profiler();
            profiler.Start();

            await ones.ParallelForEachAsync(async i =>
            {
                results.Add(i + 1);
                await Task.Delay(singleTaskDuration);
            });

            profiler.Stop();

            //assert all tasks were run
            results.Count.Should().Be(ones.Length);
            foreach (var result in results)
                result.Should().Be(2);

            //assert they were run in parallel
            profiler.Duration.Should().BeCloseTo(6.Seconds(), 5500);
        }

        [Fact]
        public void ForEach_Func_ActionIsAppliedToallElementsInCol()
        {
            const int expected = 2;

            var ones = new[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

            var twos = ones.ForEach(one => expected);

            foreach (var two in twos)
                two.Should().Be(expected);
        }

        [Fact]
        public void ForEach_Action_ActionIsAppliedToallElementsInCol()
        {
            var range = Enumerable.Range(1, 10).ToList();
            var test = Substitute.For<ITest>();

            range.ForEach(one => test.Test());

            test.Received(range.Count());
        }

        public interface ITest
        {
            void Test();
        }
    }
}
