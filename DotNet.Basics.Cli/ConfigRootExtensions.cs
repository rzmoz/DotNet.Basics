using System.Collections.Generic;
using DotNet.Basics.Sys;
using Microsoft.Extensions.Configuration;

namespace DotNet.Basics.Cli
{
    public static class ConfigRootExtensions
    {
        private static readonly string[] _emptyList = new string[0];

        public static IReadOnlyCollection<string> Environments(this IConfigurationRoot config)
        {
            return config[ArgsExtensions.EnvironmentsKey]?.Split('|') ?? _emptyList;
        }

        public static IEnumerable<string> ToArgs(this IConfigurationRoot config)
        {
            foreach (var entry in config.AsEnumerable(false))
            {
                yield return entry.Key.EnsurePrefix(ArgsExtensions.MicrosoftExtensionsArgsSwitch);
                yield return entry.Value;
            }
        }
    }
}
