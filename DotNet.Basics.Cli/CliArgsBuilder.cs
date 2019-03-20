using System;
using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Sys;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace DotNet.Basics.Cli
{
    public class CliArgsBuilder
    {
        private const char _shortExtensionsArgsSwitch = '-';
        public const string MicrosoftExtensionsArgsSwitch = "--";

        public CliArgsBuilder WithSerilog(Action<LoggerConfiguration> configureLogger = null)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.ColoredConsole();

            configureLogger?.Invoke(loggerConfiguration);
            Log.Logger = loggerConfiguration.CreateLogger();
            return this;
        }

        public CliArgs Build(string[] args, SwitchMappings switchMappings = null, Action<IConfigurationBuilder> add = null)
        {
            var configArgs = args.Select(a =>
            {
                if (a.StartsWith(_shortExtensionsArgsSwitch.ToString()))
                    a = a.TrimStart(_shortExtensionsArgsSwitch).EnsurePrefix(MicrosoftExtensionsArgsSwitch);
                return a;
            }).ToArray();

            var configBuilder = new ConfigurationBuilder()
                .AddCommandLine(configArgs, (switchMappings ?? new SwitchMappings()).ToDictionary());

            add?.Invoke(configBuilder);

            var config = configBuilder.Build();

            return new CliArgs(args, config);
        }
    }
}
