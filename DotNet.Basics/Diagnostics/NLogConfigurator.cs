using System;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Time;

namespace DotNet.Basics.Diagnostics
{

    /// <summary>
    /// https://en.wikipedia.org/wiki/Builder_pattern
    /// </summary>
    public class NLogConfigurator : IDisposable
    {
        private readonly LoggingConfiguration _config;

        public NLogConfigurator(LoggingConfiguration config = null)
        {
            _config = config ?? new LoggingConfiguration();
        }

        public void AddTarget(Target target)
        {
            AddTarget(target, "*");
        }
        public void AddTarget(Target target, string loggerNamePattern)
        {
            AddTarget(target, loggerNamePattern, LogLevel.Trace);
        }
        public void AddTarget(Target target, string loggerNamePattern,
            LogLevel logMinimumLevel)
        {
            AddTarget(target, loggerNamePattern, logMinimumLevel, LogLevel.Fatal);
        }
        public void AddTarget(Target target, string loggerNamePattern, LogLevel logMinimumLevel, LogLevel logMaximumLevel)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            if (target.Name == null)
                target.Name = target.GetType().FullName;
            _config.AddTarget(target);

            var rule = new LoggingRule(loggerNamePattern, logMinimumLevel, logMaximumLevel, target);
            _config.LoggingRules.Add(rule);
        }

        public void Build()
        {
            Build(new FastUtcTimeSource());
        }

        public void Build(TimeSource timeSource)
        {
            TimeSource.Current = timeSource;
            LogManager.Configuration = _config;
        }

        public void Dispose()
        {
            Build();
        }
    }
}
