using System;
using System.Linq;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace DotNet.Basics.Diagnostics
{
    public class InMemDiagnostics : IDiagnostics, IReadOnlyCollection<DiagnosticsEntry>
    {
        private ConcurrentQueue<DiagnosticsEntry> _entries;

        public InMemDiagnostics()
            : this(new List<DiagnosticsEntry>())
        {
        }
        public InMemDiagnostics(IEnumerable<DiagnosticsEntry> entries)
        {
            _entries = new ConcurrentQueue<DiagnosticsEntry>(entries);
        }

        public IReadOnlyCollection<T> Get<T>() where T : DiagnosticsEntry
        {
            return _entries.OfType<T>().ToList();
        }

        public IReadOnlyCollection<LogEntry> GetLogs(LogLevel logLevel)
        {
            return _entries.OfType<LogEntry>().Where(entry => entry.Level == logLevel).ToList();
        }

        public bool HasErrors()
        {
            return _entries.OfType<LogEntry>().Any(e => e.Level == LogLevel.Error);
        }

        public void Log(string message, LogLevel logLevel = LogLevel.Info, Exception exception = null)
        {
            _entries.Enqueue(new LogEntry(GetTimestamp(), message, logLevel, exception));
        }

        public void Metric(string name, double value)
        {
            _entries.Enqueue(new MetricEntry(GetTimestamp(), name, value));
        }
        
        public void Profile(string taskName, TimeSpan duration)
        {
            _entries.Enqueue(new ProfileEntry(GetTimestamp(), taskName, duration));
        }

        public IEnumerator<DiagnosticsEntry> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            Interlocked.Exchange(ref _entries, new ConcurrentQueue<DiagnosticsEntry>());
        }

        public int Count => _entries.Count;

        protected virtual DateTime GetTimestamp()
        {
            return DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"Count:{Count}";
        }
    }
}
