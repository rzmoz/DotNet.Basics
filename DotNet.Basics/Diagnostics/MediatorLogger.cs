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

        public MediatorLogger()
        {
            _loggers = new ConcurrentDictionary<string, ILogger>();
        }

        public void Add(string name, ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            _loggers.AddOrUpdate(name ?? logger.GetType().Name, logger, (n, l) => l);
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
