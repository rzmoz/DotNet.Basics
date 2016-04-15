using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Diagnostics
{
    public class MediatorDiagnostics : IDiagnostics
    {
        private ConcurrentDictionary<string, IDiagnostics> _loggers;

        public MediatorDiagnostics()
        {
            _loggers = new ConcurrentDictionary<string, IDiagnostics>();
        }

        public void Add(string name, IDiagnostics logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            _loggers.AddOrUpdate(name ?? logger.GetType().Name, logger, (n, l) => l);
        }

        public void Clear()
        {
            Interlocked.Exchange(ref _loggers, new ConcurrentDictionary<string, IDiagnostics>());
        }

        public void Log(string message, LogLevel logLevel = LogLevel.Info, Exception exception = null)
        {
            Parallel.ForEach(_loggers, logger => logger.Value.Log(message ?? string.Empty, logLevel, exception));
        }

        public void Metric(string name, double value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            Parallel.ForEach(_loggers, logger => logger.Value.Metric(name, value));
        }

        public void Profile(string taskName, TimeSpan duration)
        {
            if (taskName == null)
                throw new ArgumentNullException(nameof(taskName));
            Parallel.ForEach(_loggers, logger => logger.Value.Profile(taskName, duration));
        }
    }
}
