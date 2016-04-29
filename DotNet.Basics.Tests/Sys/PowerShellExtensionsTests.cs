using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Sys
{
    [TestFixture]
    public class PowerShellExtensionsTests
    {
        [Test]
        public void ToPSParamString_Formatting_InputIsFormattedNicely()
        {
            var input = new[] { "myStr1", "myStr2", "myStr3" };
            var formatted = input.ToPSParamString();
            formatted.Should().Be("(\"myStr1\",\"myStr2\",\"myStr3\")");
        }
    }
}
