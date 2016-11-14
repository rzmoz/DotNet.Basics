using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class StringExtensionsTests
    {
        [Fact]
        public void ToTitleCase()
        {
            var input = "PoSSeSsIon sO cOMParISon inquietude 123 HE1 he LOREM";

            var output = input.ToTitleCase();

            output.Should().Be("Possession So Comparison Inquietude 123 He1 He Lorem");
        }

        [Fact]
        public void CamelCaseTest()
        {
            var input = "PoSSeSsIon sO cOMParISon inquietude HE he LOREM";

            var output = input.ToCamelCase();

            output.Should().Be("PossessionSoComparisonInquietudeHeHeLorem");
        }
    }
}
