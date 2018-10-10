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
        public static void AddNLogging(this IServiceCollection services, Action<NLogBuilder> addTargets = null, LoggingConfiguration config = null, LogLevel minimumLogLevel = LogLevel.Trace)
        {
            using (var conf = new NLogBuilder(config))
            {
                addTargets?.Invoke(conf);
            }

            services.TryAddSingleton<ILogger, Logger<ILogger>>();
            services.AddLogging(builder => builder.AddNLog().SetMinimumLevel(minimumLogLevel));
        }

        public static void AddDotNetBasicsColoredConsoleTarget(this NLogBuilder logBuilder)
        {
            logBuilder?.AddTarget(new ColoredConsoleTarget
            {
                Layout = "${message}"
            }.WithDefaultColors());
        }
    }
}
