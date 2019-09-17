using System.Collections.Generic;
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

        public static bool IsSet(this IConfigurationRoot config,string key)
        {
            return config[key] != null;
        }
        public static bool HasValue(this IConfigurationRoot config, string key)
        {
            return string.IsNullOrWhiteSpace(config[key]) == false && config[key] != ArgsExtensions.IsSetValue;
        }
    }
}
