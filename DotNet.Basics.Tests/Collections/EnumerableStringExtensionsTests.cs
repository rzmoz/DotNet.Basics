using System.Linq;
using DotNet.Basics.Collections;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Collections
{
    public class EnumerableStringExtensionsTests
    {
        private const string _elementSomething = "myElement1";
        private const string _elementElse = "myElement2";
        readonly string[] _allPattern = { "*" };
        readonly string[] _all = { _elementSomething, _elementElse };

        [Fact]
        public void Blacklisted_AllWildcard_WildcardsAreObeyed()
        {
            _all.Length.Should().Be(2, "all length");

            //act
            var result = _all.Blacklist(_allPattern);

            //assert
            result.Any().Should().BeFalse();

        }

        [Fact]
        public void Blacklisted_MultipleWildcards_WildcardsAreObeyed()
        {
            var blacklist = new[] { "*element*".ToUpper() };//ignore case as well
            _all.Length.Should().Be(2, "all length");

            //act
            var result = _all.Blacklist(blacklist);

            //assert
            result.Any().Should().BeFalse();

        }

        [Fact]
        public void Blacklisted_WildcardEndsWith_WildcardsAreObeyed()
        {
            var blacklist = new[] { "*T1" };//ignore case
            _all.Length.Should().Be(2, "all length");
            blacklist.Length.Should().Be(1, "exclude length");

            //act
            var result = _all.Blacklist(blacklist).ToArray();

            //assert
            result.Single().Should().Be(_elementElse);
        }


        [Fact]
        public void Blacklisted_WildcardStartsWith_WildcardsAreObeyed()
        {
            var blacklist = new[] { "MY*" };//ignore case
            _all.Length.Should().Be(2, "all length");
            blacklist.Length.Should().Be(1, "exclude length");

            //act
            var result = _all.Blacklist(blacklist);

            //assert
            result.Any().Should().BeFalse();
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
        public void Whitelisted_WildcardAll_AllIsIncluded()
        {
            //act
            var result = _all.Whitelist(_allPattern).ToList();
            _all.Length.Should().Be(2);
            _allPattern.Length.Should().Be(1);

            //assert
            result.Count.Should().Be(2);
            result.First().Should().Be(_elementSomething);
            result.Last().Should().Be(_elementElse);
        }

        [Fact]
        public void Whitelisted_MultipleWildcards_WildcardsAreObeyed()
        {
            var whitelist = new[] { "*element*".ToUpper() };//ignore case as well
            _all.Length.Should().Be(2, "all length");

            //act
            var result = _all.Whitelist(whitelist).ToList();

            //assert
            result.Count.Should().Be(2);
            result.First().Should().Be(_elementSomething);
            result.Last().Should().Be(_elementElse);
        }

        [Fact]
        public void Whitelisted_WildcardEndsWith_WildcardsAreObeyed()
        {
            var whitelist = new[] { "*T1" };//ignore case
            _all.Length.Should().Be(2, "all length");
            whitelist.Length.Should().Be(1, "exclude length");

            //act
            var result = _all.Whitelist(whitelist).ToArray();

            //assert
            result.Single().Should().Be(_elementSomething);
        }


        [Fact]
        public void Whitelisted_WildcardStartsWith_WildcardsAreObeyed()
        {
            var whitelist = new[] { "MY*" };//ignore case
            _all.Length.Should().Be(2, "all length");
            whitelist.Length.Should().Be(1, "exclude length");

            //act
            var result = _all.Whitelist(whitelist).ToList();

            //assert
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
