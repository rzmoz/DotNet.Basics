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
            _loggers.AddOrUpdate(name, logger, (n, l) => l);
        }

        public void Clear()
        {
            Interlocked.Exchange(ref _loggers, new ConcurrentDictionary<string, IDiagnostics>());
        }

        public void Log(string message, LogLevel logLevel = LogLevel.Info, Exception exception = null)
        {
            Parallel.ForEach(_loggers, logger => logger.Value.Log(message, logLevel, exception));
        }

        public void Metric(string name, double value)
        {
            Parallel.ForEach(_loggers, logger => logger.Value.Metric(name, value));
        }

        public void Profile(string taskName, TimeSpan duration)
        {
            Parallel.ForEach(_loggers, logger => logger.Value.Profile(taskName, duration));
        }
    }
}
