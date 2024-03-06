using System;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Cli;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Cli
{
    public class ArgsConsoleSwitchesTests
    {
        [Theory]
        [InlineData(true, new[] { "HEADleSS" })]
        [InlineData(true, new[] { "headless" })]
        [InlineData(true, new[] { "-headless" })]
        [InlineData(true, new[] { "--headless" })]
        [InlineData(false, new[] { "--needless", "something else" })]
        public void Headless_Flag_FlagIsDetected(bool flag, string[] args)
        {
            var host = new ArgsConsoleSwitches(args);

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
            var host = new ArgsConsoleSwitches(args);

            host.IsDebug.Should().Be(flag);
        }

        [Theory]
        [InlineData(true, new[] { "DeBUG" })]
        [InlineData(true, new[] { "debug" })]
        [InlineData(true, new[] { "-debug" })]
        [InlineData(true, new[] { "--debug" })]
        [InlineData(false, new[] { "--debug", "--headless" })]//headless overrides debug
        [InlineData(false, new[] { "--headless" })]
        [InlineData(false, new[] { "" })]
        public void ShouldPauseForDebugger_Flag_FlagIsDetected(bool flag, string[] args)
        {
            var host = new ArgsConsoleSwitches(args);
            host.IsADO.Should().BeFalse(string.Join(',', args));

            host.ShouldPauseForDebugger().Should().Be(flag, string.Join(',', args));
        }
    }
}
