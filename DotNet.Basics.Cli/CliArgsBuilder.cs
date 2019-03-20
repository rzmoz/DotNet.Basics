using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace DotNet.Basics.Cli
{
    public class CliArgsBuilder
    {
        private const string _microsoftExtensionsArgsSwitch = "--";

        public CliArgs Build(string[] args, Action<IConfigurationBuilder> add = null, string argsSwitch = "-")
        {
            var configArgs = args.Select(a =>
            {
                if (a.StartsWith(argsSwitch))
                    a = $"{_microsoftExtensionsArgsSwitch}{a.Substring(argsSwitch.Length)}";
                return a;
            }).ToArray();

            var configBuilder = new ConfigurationBuilder()
                .AddCommandLine(configArgs);

            add?.Invoke(configBuilder);

            var config = configBuilder.Build();

            return new CliArgs(args, config, argsSwitch);
        }
    }
}
