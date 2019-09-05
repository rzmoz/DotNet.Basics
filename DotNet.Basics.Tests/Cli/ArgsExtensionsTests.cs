using System.Linq;
using DotNet.Basics.Cli;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Cli
{
    public class ArgsExtensionsTests
    {
        [Theory]
        [InlineData("", 0)]
        [InlineData("-myflag", 2)]
        [InlineData("-myflag|IsSet", 2)]
        [InlineData("-myflag1|/myflag2", 4)]
        public void EnsureFlagsHaveValue_Parse_FlagsHaveValue(string argsString, int expectedLength)
        {
            var formatted = argsString.Split('|').EnsureFlagsHaveValue();
            formatted.Count().Should().Be(expectedLength);
        }

        [Theory]
        [InlineData("Env1|Env1", 1)]
        [InlineData("Env1|eNV1", 1)]//case insensitive
        [InlineData("my env", 1)]
        [InlineData("env1|env2", 2)]
        public void EnsureEnvironmentsAreDistinct_DetectEnvironments_EnvironmentsAreDistinct(string environmentsValue, int expectedEnvironments)
        {
            var args = new[] { $"{ArgsExtensions.MicrosoftExtensionsArgsSwitch}{ArgsExtensions.EnvironmentsKey}", environmentsValue };

            var formatted = args.EnsureEnvironmentsAreDistinct().Where(arg => arg.IsArgSwitch() == false).ToList();

            formatted.Single().Split('|').Count().Should().Be(expectedEnvironments);
        }

        [Theory]
        [InlineData("-myflag1|/myflag2|--myFlag3")]
        public void CleanArgsForCli_NormalizeSwitch_SwitchesAreNormalized(string argsString)
        {
            var args = argsString.Split('|');

            var formatted = args.CleanArgsForCli();

            var switches = formatted.Where(arg => arg.StartsWith('-')).ToList();

            switches.Count().Should().Be(args.Length);//ensure we test all switches
            switches.All(arg => arg.StartsWith(ArgsExtensions.MicrosoftExtensionsArgsSwitch)).Should().BeTrue();
        }
    }
}
