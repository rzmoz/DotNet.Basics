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

        public void Log(IEnumerable<LogEntry> entries)
        {
            if (entries == null)
                return;
            foreach (var logEntry in entries)
                _entries.Enqueue(logEntry);
        }
        public void Log(params LogEntry[] entries)
        {
            Log((IEnumerable<LogEntry>)entries);
        }

        public IReadOnlyCollection<LogEntry> Get(LogLevel logLevel)
        {
            return _entries.Where(entry => entry.Level == logLevel).ToList();
        }

        public IReadOnlyCollection<LogEntry> Entries => _entries.ToList();

        public void Clear()
        {
            Interlocked.Exchange(ref _entries, new ConcurrentQueue<LogEntry>());
        }

        public int Count => _entries.Count;

        public bool Failed => _entries.Any(e => e.Level >= LogLevel.Error);

        public IEnumerator<LogEntry> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return $"Count:{Count};Debugs:{Get(LogLevel.Debug).Count};Traces:{Get(LogLevel.Trace).Count};Infos:{Get(LogLevel.Information).Count};Warnings:{Get(LogLevel.Warning).Count};Errors:{Get(LogLevel.Error).Count};Criticals:{Get(LogLevel.Critical).Count}";
        }

        protected override void Log(LogEntry entry)
        {
            if (entry == null)
                return;
            _entries.Enqueue(entry);
        }
    }
}
