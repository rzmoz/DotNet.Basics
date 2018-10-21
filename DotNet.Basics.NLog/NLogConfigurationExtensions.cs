using System;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Time;

namespace DotNet.Basics.NLog
{
    public static class NLogConfigurationExtensions
    {
        public static LoggingConfiguration AddColoredConsoleTarget(this LoggingConfiguration config)
        {
            config.AddTarget(new ColoredConsoleTarget
            {
                Layout = "${message}"
            }.WithDefaultColors(), null);
            return config;
        }

        public static LoggingConfiguration AddTarget(this LoggingConfiguration config, Target target, Action<LoggingRule> configureLoggingRule = null)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            if (target.Name == null)
                target.Name = target.GetType().FullName;
            config.AddTarget(target);

            var rule = new LoggingRule("*", target);
            rule.EnableLoggingForLevels(LogLevel.Trace, LogLevel.Fatal);
            configureLoggingRule?.Invoke(rule);
            config.LoggingRules.Add(rule);
            return config;
        }

        public static void ConfigureNLogging(this LoggingConfiguration config, Action<LoggingConfiguration> configureNLog = null, TimeSource timeSource = null)
        {
            TimeSource.Current = timeSource ?? new FastUtcTimeSource();
            configureNLog?.Invoke(config);
        }
    }
}
