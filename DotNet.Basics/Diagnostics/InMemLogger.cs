using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public class InMemLogger : DotNetBasicsLogger, IReadOnlyCollection<LogEntry>
    {
        private ConcurrentQueue<LogEntry> _entries;

        public LogLevel LogLevel { get; set; }

        public InMemLogger()
            : this(new List<LogEntry>())
        {
        }
        public InMemLogger(IEnumerable<LogEntry> entries)
        {
            _entries = new ConcurrentQueue<LogEntry>(entries);
            LogLevel = LogLevel.Debug;
        }

        public IReadOnlyCollection<T> Get<T>() where T : LogEntry
        {
            return _entries.OfType<T>().ToList();
        }

        public IReadOnlyCollection<LogEntry> GetLogs(LogLevel logLevel)
        {
            return _entries.Where(entry => entry.Level == logLevel).ToList();
        }

        public bool HasFailed()
        {
            return _entries.Any(e => e.Level == LogLevel.Error || e.Level == LogLevel.Critical);
        }

        public IEnumerator<LogEntry> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            Interlocked.Exchange(ref _entries, new ConcurrentQueue<LogEntry>());
        }

        public int Count => _entries.Count;

        public override string ToString()
        {
            return $"Count:{Count};Failed:{HasFailed()}";
        }

        protected override void Log(LogEntry entry)
        {
            if (entry == null)
                return;
            _entries.Enqueue(entry);
        }
    }
}
