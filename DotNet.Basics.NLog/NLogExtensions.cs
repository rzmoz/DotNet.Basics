using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;

namespace DotNet.Basics.NLog
{
    public static class NLogExtensions
    {
        public static void AddNLogging(this IServiceCollection services, Action<NLogConfigurator> addTargets = null, LoggingConfiguration config = null, LogLevel minimumLogLevel = LogLevel.Trace)
        {
            using (var conf = new NLogConfigurator(config))
            {
                if (addTargets == null)
                    conf.AddTarget(new ColoredConsoleTarget().WithDefaultColors());
                else
                    addTargets(conf);
            }

            services.TryAddSingleton<ILogger, Logger<ILogger>>();
            services.AddLogging(builder => builder.AddNLog().SetMinimumLevel(minimumLogLevel));
        }
    }
}
