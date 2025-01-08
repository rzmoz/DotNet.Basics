using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using DotNet.Basics.Sys.Text;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys.Text
{
    public class SysRegexTests
    {
        private const string _sampleInput = "Lorem ipsum odor amet, consectetuer adipiscing elit.";

        [Fact]
        public void Ctor_Implicit_RegexIsInstantiatedFromPatternString()
        {
            //compiles
            SysRegex regex = _sampleInput;
        }

        [Fact]
        public void TryMatch_Simple_MatchReturnsGroup1Value()
        {
            //arrange
            SysRegex regex = "Lorem (ipsum) odor";

            //act
            var isMatch = regex.TryMatch(_sampleInput, out var match);

            //assert
            isMatch.Should().BeTrue();
            match.Should().Be("ipsum");
        }

        [Fact]
        public void TryMatch_NoMatch_FallbackValueIsReturned()
        {
            //arrange
            var fallbackValue = "§§123123123§§";
            SysRegex regex = "this pattern is not found in lorem ipsum text";

            //act
            var isMatch = regex.TryMatch(_sampleInput, out var match, fallbackValue);

            //assert
            isMatch.Should().BeFalse();
            match.Should().Be(fallbackValue);
        }

        [Fact]
        public void TryMatch_GroupNumber_SpecifiedGroupNumberIsReturned()
        {
            //arrange
            SysRegex regex = "Lorem (ipsum) (odor) amet";

            //act
            var isMatch = regex.TryMatch(_sampleInput, out var match, groupNumber: 2);

            //assert
            isMatch.Should().BeTrue();
            match.Should().Be("odor");
        }

        [Fact]
        public void Match_Simple_MatchReturnsGroup1Value()
        {
            //arrange
            SysRegex regex = "Lorem (ipsum) odor";

            //act
            var matchValue = regex.Match(_sampleInput);

            //assert
            matchValue.Should().Be("ipsum");
        }

        [Fact]
        public void Match_NoMatch_FallbackValueIsReturned()
        {
            //arrange
            var fallbackValue = "§§123123123§§";
            SysRegex regex = "this pattern is not found in lorem ipsum text";

            //act
            var matchValue = regex.Match(_sampleInput, fallbackValue);

            //assert
            matchValue.Should().Be(fallbackValue);
        }

        [Fact]
        public void Match_GroupNumber_SpecifiedGroupNumberIsReturned()
        {
            //arrange
            SysRegex regex = "Lorem (ipsum) (odor) amet";

            //act
            var matchValue = regex.Match(_sampleInput, groupNumber: 2);

            //assert
            matchValue.Should().Be("odor");
        }

        [Theory]
        [InlineData("Lorem", true)]
        [InlineData("Not a match", false)]
        public void IsMatch_Pattern_IsMatchMatchesExpectations(string pattern, bool expectedIsMatch)
        {
            //arrange
            SysRegex regex = pattern;

            //act
            var isMatch = regex.Test(_sampleInput);

            //assert
            isMatch.Should().Be(expectedIsMatch);
        }

        [Fact]
        public void Matches_MatchProcessing_MatchCollectionIsRetrieved()
        {
            //arrange
            SysRegex regex = "(o)";

            //act
            MatchCollection matches = regex.Matches(_sampleInput);

            //assert
            matches.Count.Should().Be(4);
        }

        [Fact]
        public void Replace_StringReplacement_MatchIsReplaced()
        {
            //arrange
            var replacement = "ice-cream";
            SysRegex regex = "Lorem";

            //act
            var result = regex.Replace(_sampleInput, replacement);

            //assert
            result.Should().Be(replacement + " ipsum odor amet, consectetuer adipiscing elit.");
        }

        [Fact]
        public void Remove_RemovesEntireMatches_RemoveIsRemoved()
        {
            //arrange

            SysRegex regex = " ipsum (odor) amet";

            //act
            var result = regex.Remove(_sampleInput);

            //assert
            result.Should().Be("Lorem, consectetuer adipiscing elit.");
        }

        [Fact]
        public void Remove_MultipleRegexes_AlleRegexesAreInvokedInOrder()
        {
            //arrange
            SysRegex[] regexes = ["ip", "o", @"[0-9a]*"];

            var input = "ips1um (odor) amet234876234872634876234";

            //act
            var result = input.Remove(regexes);

            //assert
            result.Should().Be("sum (dr) met");
        }
    }
}