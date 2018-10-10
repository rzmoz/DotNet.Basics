using System;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Time;

namespace DotNet.Basics.NLog
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Builder_pattern
    /// </summary>
    public class NLogBuilder : IDisposable
    {
        /// <summary>
        /// Remember to call Build() upon completion to set configuration to global NLog LogManager configuration
        /// </summary>
        /// <param name="config"></param>
        public NLogBuilder(LoggingConfiguration config = null)
        {
            Config = config ?? new LoggingConfiguration();
        }

        public LoggingConfiguration Config { get; }

        public void AddTarget(Target target)
        {
            AddTarget(target, "*");
        }

        public void AddTarget(Target target, string loggerNamePattern)
        {
            AddTarget(target, loggerNamePattern, LogLevel.Trace);
        }

        public void AddTarget(Target target, string loggerNamePattern, LogLevel logMinimumLevel)
        {
            AddTarget(target, loggerNamePattern, logMinimumLevel, LogLevel.Fatal);
        }

        public void AddTarget(Target target, string loggerNamePattern, LogLevel logMinimumLevel, LogLevel logMaximumLevel)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            if (target.Name == null)
                target.Name = target.GetType().FullName;
            Config.AddTarget(target);

            var rule = new LoggingRule(loggerNamePattern, logMinimumLevel, logMaximumLevel, target);
            Config.LoggingRules.Add(rule);
        }

        /// <summary>
        /// Be aware that will set the global NLog logManager configuration. Only set this once during app initialization
        /// </summary>
        public void Build()
        {
            Build(new FastUtcTimeSource());
        }

        /// <summary>
        /// Be aware that will set the global NLog logManager configuration. Only set this once during app initialization
        /// </summary>
        /// <param name="timeSource"></param>
        public void Build(TimeSource timeSource)
        {
            TimeSource.Current = timeSource;
            LogManager.Configuration = Config;
        }

        public void Dispose()
        {
            Build();
        }
    }
}
