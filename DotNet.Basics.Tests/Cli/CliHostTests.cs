using DotNet.Basics.Cli;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Cli
{
    public class CliHostTests
    {
        [Theory]
        [InlineData(true, new[] { "HEADleSS" })]
        [InlineData(true, new[] { "headless" })]
        [InlineData(true, new[] { "-headless" })]
        [InlineData(true, new[] { "--headless" })]
        [InlineData(false, new[] { "--needless", "something else" })]
        public void Headless_Flag_FlagIsDetected(bool flag, string[] args)
        {
            var host = new CliHost(args);

            host.IsHeadless.Should().Be(flag);
        }

        [Theory]
        [InlineData(true, new[] { "DeBUG" })]
        [InlineData(true, new[] { "debug" })]
        [InlineData(true, new[] { "-debug" })]
        [InlineData(true, new[] { "--debug" })]
        [InlineData(false, new[] { "--budge", "something else" })]
        public void Debug_Flag_FlagIsDetected(bool flag, string[] args)
        {
            var host = new CliHost(args);

            host.IsDebug.Should().Be(flag);
        }

        [Theory]
        [InlineData(true, new[] { "DeBUG" })]
        [InlineData(true, new[] { "debug" })]
        [InlineData(true, new[] { "-debug" })]
        [InlineData(true, new[] { "--debug" })]
        [InlineData(false, new[] { "--debug", "--headless" })]
        [InlineData(false, new[] { "--headless" })]
        [InlineData(false, new[] { "" })]
        public void ShouldPauseForDebugger_Flag_FlagIsDetected(bool flag, string[] args)
        {
            var host = new CliHost(args);

            host.ShouldPauseForDebugger().Should().Be(flag);
        }
    }
}
