using System;
using DotNet.Basics.Cli;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Cli
{
    public class CliHostBuilderTests
    {
        [Fact]
        public void Build_Args_ArePassed()
        {
            var value = "myValue";
            string[] args = { $"-{nameof(TestCliHost.MySetting)}", value };

            var host = new CliHostBuilder<TestArgs>(args)
                .Build();

            host[nameof(TestCliHost.MySetting)].Should().Be(value);
        }

        [Fact]
        public void Build_HydratorArgs_ArePassed()
        {
            var keyValue = "myValue";
            var boolValue = true;
            var enumValue = LogLevel.Info;
            var dirPathValue = "c:\\myDir".ToDir();
            var filePathValue = "c:\\myPath".ToFile();
            var stringListValue = "Hello|World!";

            string[] args =
            {
                $"-{nameof(TestArgs.Key)}", keyValue,
                $"-{nameof(TestArgs.Boolean)}", boolValue.ToString(),
                $"-{nameof(TestArgs.Enum)}", enumValue.ToName(),
                $"-{nameof(TestArgs.DirPath)}", dirPathValue.RawPath,
                $"-{nameof(TestArgs.FilePath)}", filePathValue.RawPath,
                $"-{nameof(TestArgs.StringList)}", stringListValue,
            };

            var host = new CliHostBuilder<TestArgs>(args)
                .Build();

            host.Args.Key.Should().Be(keyValue);
            host.Args.Boolean.Should().Be(boolValue);
            host.Args.Enum.Should().Be(enumValue);
            host.Args.DirPath.Should().Be(dirPathValue);
            host.Args.FilePath.Should().Be(filePathValue);
            host.Args.StringList.Count.Should().Be(2);
        }


        [Fact]
        public void Ctor_AutoHydrateArgsFromConfig_ArgsAreHydrated()
        {
            var value = "HelloWorld!";
            string[] args = { $"-{nameof(TestArgs.Key)}", value };
            var host = new CliHostBuilder<TestArgs>(args)
                .Build();

            host.Args.Key.Should().Be(value);
        }

        [Fact]
        public void BuildCustomHost_Args_ArePassed()
        {
            var value = "myValue";
            string[] args = { $"-{nameof(TestCliHost.MySetting)}", value };

            var host = new CliHostBuilder<TestArgs>(args)
                .BuildCustomHost((config, log) => new TestCliHost(new TestArgs(), args, config, log));

            host.MySetting.Should().Be(value);
        }

        [Theory]
        [InlineData("config")]
        [InlineData("configuration")]
        public void SwitchMappings_LookupValueByMapping_ValueIsFound(string argsKey)
        {
            var mainKey = "configuration";
            var value = "myValue";
            var inputArgs = new[] { $"--{argsKey}", value };


            var args = new CliHostBuilder<EventArgs>(inputArgs)
                .WithArgsSwitchMappings(mappings =>
                {
                    mappings.Add(argsKey, mainKey);
                })
                .Build();

            args[mainKey].Should().Be(value);
        }

        [Theory]
        [InlineData("key", "myValue")]
        public void GetByPosition_KeyNoySet_ValueIsFound(string key, string value)
        {
            var args = new[] { value };

            var cliArgs = new CliHostBuilder<EventArgs>(args).Build();
            cliArgs[key].Should().BeNull();//key not set
            cliArgs[key, 0].Should().Be(value);//found by position
        }

        [Theory]
        [InlineData("debug", "debug", true)]
        [InlineData("d", "debug", true)]
        [InlineData("v", "nvvvvvv", false)]
        public void IsSet_Flag_IsFound(string arg, string flag, bool isSet)
        {
            var args = new[] { arg.EnsurePrefix("-") };
            args.IsSet(flag, true).Should().Be(isSet);
        }

        [Theory]
        [InlineData("myKey", "myValue")]
        public void Index_FindByKey_ValueIsFound(string key, string value)
        {
            var args = new[] { key.EnsurePrefix("-"), value };

            var cliArgs = new CliHostBuilder<EventArgs>(args).Build();

            args.IsSet(key).Should().BeTrue();
            cliArgs.Config[key].Should().NotBeNull();
            cliArgs[key].Should().Be(cliArgs.Config[key]);
            cliArgs[key].Should().Be(value);
        }
        [Fact]
        public void Index_FindByIndex_ValueIsFound()
        {
            var pos1 = "HelloWorld!";
            var args = new[] { "pos0", pos1, "pos2" };

            var cliArgs = new CliHostBuilder<EventArgs>(args).Build();

            cliArgs[1].Should().Be(pos1);
        }
    }
}
