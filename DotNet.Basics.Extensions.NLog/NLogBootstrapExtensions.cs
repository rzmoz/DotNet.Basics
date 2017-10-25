using System;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;

namespace DotNet.Basics.Extensions.NLog
{
    public static class NLogBootstrapExtensions
    {
        public static void AddNLog(this ILoggingBuilder factory, Action<NLogConfigurator> configurationActions, LoggingConfiguration config = null)
        {
            factory.AddNLog(null, configurationActions, config);
        }

        public static void AddNLog(this ILoggingBuilder factory, NLogProviderOptions options, Action<NLogConfigurator> configurationActions, LoggingConfiguration config = null)
        {
            using (var configurator = new NLogConfigurator(config ?? new LoggingConfiguration()))
            {
                configurationActions?.Invoke(configurator);
                configurator.Build();
            }

            factory.AddNLog(options);
        }
    }
}
