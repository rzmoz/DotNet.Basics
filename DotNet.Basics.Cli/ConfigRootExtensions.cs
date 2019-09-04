using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace DotNet.Basics.Cli
{
    public static class ConfigRootExtensions
    {
        private static readonly IReadOnlyList<string> _emptyList = new List<string>(0);

        public static IReadOnlyList<string> Environments(this IConfigurationRoot config)
        {
            return config[ArgsSwitchMappings.EnvironmentsKey]?.Split('|').Select(env => env.Trim())?.ToList() ?? _emptyList;
        }
    }
}
