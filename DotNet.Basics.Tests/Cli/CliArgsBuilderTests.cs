using System;
using System.Collections.Generic;
using System.Text;
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

            var mappings = new SwitchMappings
            {
                {argsKey,mainKey }
            };

            var args = new CliArgsBuilder().Build(inputArgs, mappings);

            args[mainKey].Should().Be(value);
        }
    }
}
