using System.Linq;
using System.Text.RegularExpressions;
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
