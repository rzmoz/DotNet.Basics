using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public class MediatorLogger : DotNetBasicsLogger
    {
        private ConcurrentDictionary<string, ILogger> _loggers;

        public MediatorLogger(params ILogger[] loggers)
        {
            _loggers = new ConcurrentDictionary<string, ILogger>();
            foreach (var logger in loggers)
                Add(logger);
        }

        public void Add(ILogger logger, string name = null)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            _loggers.AddOrUpdate(name ?? Guid.NewGuid().ToString(), logger, (n, l) => l);
        }

        public void Clear()
        {
            Interlocked.Exchange(ref _loggers, new ConcurrentDictionary<string, ILogger>());
        }

        public override void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            ConcurrentQueue<Exception> exceptions = null;
            Parallel.ForEach(_loggers, logger =>
            {
                try
                {
                    logger.Value.Log(logLevel, eventId, state, exception, formatter);
                }
                catch (Exception ex)
                {
                    if (exceptions == null)
                    {
                        exceptions = new ConcurrentQueue<Exception>();
                    }

                    exceptions.Enqueue(ex);
                }
            });

            if (exceptions?.Count > 0)
                throw new AggregateException("An error occurred while writing to logger(s).", exceptions);

        }

        protected override void Log(LogEntry entry)
        {
            //not implemented
        }
    }
}
