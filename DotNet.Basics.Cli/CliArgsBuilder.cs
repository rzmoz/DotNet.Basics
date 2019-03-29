using System;
using System.Linq;
using DotNet.Basics.Serilog;
using DotNet.Basics.Sys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DotNet.Basics.Cli
{
    public class CliArgsBuilder
    {
        public const char DefaultArgsSwitch = '-';
        public const string MicrosoftExtensionsArgsSwitch = "--";
        public SwitchMappings SwitchMappings { get; } = new SwitchMappings();

        public CliArgsBuilder WithSerilog(Action<LoggerConfiguration> config = null, LogLevel minimumLevel = LogLevel.Trace)
        {
            Diagnostics.Log.Logger.WithSerilog(conf =>
            {
                conf.WriteTo.ColoredConsole();
                config?.Invoke(conf);
            }, minimumLevel);
            return this;
        }

        public CliArgsBuilder WithSwitchMappings(Func<SwitchMappings> switchMappings)
        {
            SwitchMappings.AddRange(switchMappings?.Invoke());
            return this;
        }

        public CliArgs Build(string[] args, Action<IConfigurationBuilder> add = null)
        {
            var configArgs = args.Select(a =>
            {
                if (a.StartsWith(DefaultArgsSwitch.ToString()))
                    a = a.TrimStart(DefaultArgsSwitch).EnsurePrefix(MicrosoftExtensionsArgsSwitch);
                return a;
            }).ToArray();

            var configBuilder = new ConfigurationBuilder()
                .AddCommandLine(configArgs, SwitchMappings.ToDictionary());

            add?.Invoke(configBuilder);

            var config = configBuilder.Build();

            return new CliArgs(args, config);
        }
    }
}
