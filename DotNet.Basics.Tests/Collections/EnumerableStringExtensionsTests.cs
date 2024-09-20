using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DotNet.Basics.Collections;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Collections
{
    public class EnumerableStringExtensionsTests
    {
        private const string _elementSomething = "myElement1";
        private const string _elementElse = "myElement2";
        readonly Regex _allSearchRegex = new(".+");
        
        readonly string[] _all = [_elementSomething, _elementElse];

        [Fact]
        public void Concat_WithSingleElement_EnumerableIsConcatenated()
        {
            var range = 7;
            var series = Enumerable.Range(1, range).ToList();

            var concatenatedSingleElementLast = series.Concat(6);
            var concatenatedSingleElementFirst = 456.Concat(series);

            concatenatedSingleElementFirst.Count().Should().Be(range + 1);
            concatenatedSingleElementLast.Count().Should().Be(range + 1);
        }
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
            });

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
            });

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
            });

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

        [Fact]
        public void Blacklisted_ExplicitMatch_MatchIsBlacklisted()
        {
            var blacklist = new[] { _elementSomething };
            _all.Length.Should().Be(2);
            blacklist.Length.Should().Be(1);

            //act
            var result = _all.Blacklist(blacklist);

            //assert
            result.Single().Should().Be(_elementElse);
        }

        [Fact]
        public void Blacklisted_ExplicitMatchNotFound_NothingIsBlacklisted()
        {
            var exclude = new[] { "SomethingElse" };
            _all.Length.Should().Be(2);
            exclude.Length.Should().Be(1);

            //act
            var result = _all.Blacklist(exclude).ToList();

            //assert
            result.Count.Should().Be(2);
            result.First().Should().Be(_elementSomething);
            result.Last().Should().Be(_elementElse);
        }

        [Fact]
        public void Whitelist_Regex_AllIsIncluded()
        {
            //act
            var result = _all.Whitelist(_allSearchRegex).ToList();

            //assert
            _all.Length.Should().Be(2);
            result.Count.Should().Be(2);
            result.First().Should().Be(_elementSomething);
            result.Last().Should().Be(_elementElse);
        }

        [Fact]
        public void Whitelisted_ExactMatch_MatchIsIncluded()
        {
            var includeWithAlreadyIncludedElements = new[] { _elementSomething };
            includeWithAlreadyIncludedElements.Length.Should().Be(1);

            //act
            var result = _all.Whitelist(includeWithAlreadyIncludedElements);

            //assert
            result.Single().Should().Be(_elementSomething);
        }

        [Fact]
        public void Whitelisted_ExactMatchNotFound_NothingIsReturned()
        {
            var newIncludeElement = "NewInclude";
            var newIncludes = new[] { newIncludeElement };

            _all.Length.Should().Be(2);
            newIncludes.Length.Should().Be(1);

            //act
            var result = _all.Whitelist(newIncludes);

            //assert
            result.Any().Should().BeFalse();
        }
    }
}
