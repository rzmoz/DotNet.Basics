using System;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace DotNet.Basics.Diagnostics
{
    public class LoggingConfigurationBuilder : IDisposable
    {
        private readonly LoggingConfiguration _config;
        
        public LoggingConfigurationBuilder(LoggingConfiguration config = null)
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
            LogManager.Configuration = _config;
        }

        public void Dispose()
        {
            Build();
        }
    }
}
