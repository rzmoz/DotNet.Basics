using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace DotNet.Basics.Cli
{
    public class CliArgsBuilder
    {
        private const string _microsoftExtensionsArgsSwitch = "--";

        public CliArgsBuilder WithSerilog(Action<LoggerConfiguration> configureLogger = null)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.ColoredConsole();

            configureLogger?.Invoke(loggerConfiguration);
            Log.Logger = loggerConfiguration.CreateLogger();
            return this;
        }

        public CliArgs Build(string[] args, IDictionary<string, string> switchMappings = null, Action<IConfigurationBuilder> add = null, string argsSwitch = "-")
        {
            var configArgs = args.Select(a =>
            {
                if (a.StartsWith(argsSwitch))
                    a = $"{_microsoftExtensionsArgsSwitch}{a.Substring(argsSwitch.Length)}";
                return a;
            }).ToArray();

            var configBuilder = new ConfigurationBuilder()
                .AddCommandLine(configArgs, switchMappings ?? new Dictionary<string, string>());

            add?.Invoke(configBuilder);

            var config = configBuilder.Build();

            return new CliArgs(args, config, argsSwitch);
        }
    }
}
