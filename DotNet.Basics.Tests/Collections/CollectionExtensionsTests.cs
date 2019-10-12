using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Collections;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Collections
{
    public class CollectionExtensionsTests
    {
        [Fact]
        public void None_EmptyList_NoneFound()
        {
            var ints = new int[0];

            ints.None().Should().BeTrue();
            ints.None(i => i == 0).Should().BeTrue();
            ints.None(i => i != 0).Should().BeTrue();
        }

        [Fact]
        public void None_FilledList_NoneFound()
        {
            const int expected = 1;

            var ones = new[] { expected, expected, expected, expected, expected, expected };

            ones.None().Should().BeFalse();
            ones.None(o => o == expected).Should().BeFalse();
            ones.None(o => o != expected).Should().BeTrue();
        }

        [Fact]
        public void Contains_Predicated_NoneFound()
        {
            const int expected = 1;

            var ones = new[] { expected, expected, expected, expected, expected, expected };

            ones.Contains(o => o == expected).Should().BeTrue();
            ones.Contains(o => o != expected).Should().BeFalse();
        }

        [Fact]
        public void ForEachParallel_ApplyAction_ActionIsAppliedToAllElements()
        {
            var range = GetTestRange();

            var results = range.ForEachParallel(test => test.IncreaseValue());

            results.All(result => result.Value == 1).Should().BeTrue();
        }

        [Fact]
        public void ForEachParallel_ApplyFunc_FuncIsAppliedToAllElements()
        {
            var range = GetTestRange();

            var results = range.ForEachParallel(test =>
            {
                test.IncreaseValue();
                return test.Value;
            });

            results.All(result => result == 1).Should().BeTrue();
        }

        [Fact]
        public async Task ForEachParallelAsync_ApplyAction_ActionIsAppliedToAllElements()
        {
            var range = GetTestRange();

            var results = await range.ForEachParallelAsync(test =>
            {
                test.IncreaseValue();
                return Task.CompletedTask;
            }).ConfigureAwait(false);

            results.All(result => result.Value == 1).Should().BeTrue();
        }

        [Fact]
        public async Task ForEachParallelAsync_ApplyFunc_FuncIsAppliedToAllElements()
        {
            var range = GetTestRange();

            var results = await range.ForEachParallelAsync(test =>
            {
                test.IncreaseValue();
                return Task.FromResult(test.Value);
            }).ConfigureAwait(false);

            results.All(result => result == 1).Should().BeTrue();
        }

        [Fact]
        public void ForEach_ApplyAction_ActionIsAppliedToAllElements()
        {
            var range = GetTestRange();

            var results = range.ForEach(test => test.IncreaseValue());

            results.All(result => result.Value == 1).Should().BeTrue();
        }

        [Fact]
        public void ForEach_ApplyFunc_FuncIsAppliedToAllElements()
        {
            var range = GetTestRange();

            var results = range.ForEach(test =>
            {
                test.IncreaseValue();
                return test.Value;
            });

            results.All(result => result == 1).Should().BeTrue();
        }

        [Fact]
        public async Task ForEachAsync_ApplyAction_ActionIsAppliedToAllElements()
        {
            var range = GetTestRange();

            var results = await range.ForEachAsync(test =>
            {
                test.IncreaseValue();
                return Task.CompletedTask;
            });

            results.All(result => result.Value == 1).Should().BeTrue();
        }

        [Fact]
        public async Task ForEachAsync_ApplyFunc_FuncIsAppliedToAllElements()
        {
            var range = GetTestRange();

            var results = await range.ForEachAsync(test =>
            {
                test.IncreaseValue();
                return Task.FromResult(test.Value);
            }).ConfigureAwait(false);

            results.All(result => result == 1).Should().BeTrue();
        }

        private ICollection<ForEachTest> GetTestRange()
        {
            var testRange = Enumerable.Range(1, 10).Select(i => new ForEachTest()).ToList();
            testRange.All(test => test.Value == 0).Should().BeTrue();
            return testRange;
        }

        public class ForEachTest
        {
            public int Value { get; private set; }

            public void IncreaseValue()
            {
                Value++;
            }
        }
    }
}
