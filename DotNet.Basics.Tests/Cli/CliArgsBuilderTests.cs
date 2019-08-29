using DotNet.Basics.Cli;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Cli
{
    public class CliArgsBuilderTests
    {
        [Theory]
        [InlineData("config")]
        [InlineData("configuration")]
        public void SwitchMappings_LookupValueByMapping_ValueIsFound(string argsKey)
        {
            var mainKey = "configuration";
            var value = "myValue";
            var inputArgs = new[] { $"--{argsKey}", value };


            var args = new CliHostBuilder(inputArgs, () => new ArgsSwitchMappings
                {
                    {argsKey, mainKey}
                })
                .Build();

            args[mainKey].Should().Be(value);
        }
    }
}
