using System;
using System.Linq;
using DotNet.Basics.Cli;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Cli
{
    public class CliHostTests
    {
        [Fact]
        public void Verbose_NotSet_VerboseIsFalse()
        {
            var args = new[] { "something" };

            var host = new CliHostBuilder(args).Build();

            host.Verbose.Should().BeFalse();
        }
        [Theory]
        [InlineData("Verbose")]//spelled on point
        [InlineData("verbose")]//all lower case
        [InlineData("VERBOSE")]//all upper case
        [InlineData("vErBOSe")]//mixed casing
        public void Verbose_IsSet_VerboseIsTrue(string arg)
        {
            var args = new[] { $"-{arg}" };

            var host = new CliHostBuilder(args).Build();

            host.Verbose.Should().BeTrue();
        }

        [Fact]
        public void Configuration_AppSettingsJson_ConfigurationIsEnvironmentSpecific()
        {
            var environment = "test";
            var args = new[] { "-envs", environment };//should make config look for appsettings.test.json

            var host = new CliHostBuilder(args).Build();

            host.Environments.Contains(environment, StringComparer.InvariantCultureIgnoreCase).Should().BeTrue();
            host["settingFrom"].Should().Be("appSettings.test.json");
            host["hello"].Should().Be("Test World!");
        }

        [Fact]
        public void Environments_Order_OrderIsKept()
        {
            var environment1 = "lorem";
            var environment2 = "ipsum";
            var environment3 = "golem";

            var args = new[] { "-envs", $"{environment1}|{environment2}|{environment3}" };

            var host = new CliHostBuilder(args).Build();

            //order must be kept
            host.Environments.First().Should().Be(environment1.ToTitleCase());
            host.Environments.Skip(1).Take(1).Single().Should().Be(environment2.ToTitleCase());
            host.Environments.Last().Should().Be(environment3.ToTitleCase());
        }
    }
}
