using System;
using System.Linq;
using DotNet.Basics.Sys;
using Microsoft.Extensions.Configuration;

namespace DotNet.Basics.Cli
{
    public class CliHostBuilder
    {
        public const char DefaultArgsSwitch = '-';
        public const string MicrosoftExtensionsArgsSwitch = "--";
        public SwitchMappings SwitchMappings { get; } = new SwitchMappings();

        public CliHostBuilder WithSwitchMappings(Func<SwitchMappings> switchMappings)
        {
            SwitchMappings.AddRange(switchMappings?.Invoke());
            return this;
        }

        public CliHostBuilder WithColoredConsole(bool includeTimestamp = false, ColoredConsoleTheme consoleTheme = null)
        {
            var coloredConsole = new ColoredConsoleWriter(includeTimestamp, consoleTheme);
            Diagnostics.Log.Logger.MessageLogged += coloredConsole.Write;
            return this;
        }

        public CliHost Build(string[] args, Action<IConfigurationBuilder> add = null)
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


            return new CliHost(args, config, Diagnostics.Log.Logger);
        }
    }
}
