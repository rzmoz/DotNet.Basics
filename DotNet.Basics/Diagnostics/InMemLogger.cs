using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public class InMemLogger : DotNetBasicsLogger
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

        public IReadOnlyCollection<T> Get<T>() where T : LogEntry
        {
            return _entries.OfType<T>().ToList();
        }

        public IReadOnlyCollection<LogEntry> GetLogs(LogLevel logLevel)
        {
            return _entries.Where(entry => entry.Level == logLevel).ToList();
        }

        public IReadOnlyCollection<LogEntry> Entries => _entries;

        public void Clear()
        {
            Interlocked.Exchange(ref _entries, new ConcurrentQueue<LogEntry>());
        }

        public int Count => _entries.Count;

        public override string ToString()
        {
            return $"Count:{Count};Debugs:{GetLogs(LogLevel.Debug).Count};Verboses:{GetLogs(LogLevel.Verbose).Count};Infos:{GetLogs(LogLevel.Information).Count};Warnings:{GetLogs(LogLevel.Warning).Count};Errors:{GetLogs(LogLevel.Error).Count};Criticals:{GetLogs(LogLevel.Critical).Count}";
        }

        protected override void Log(LogEntry entry)
        {
            if (entry == null)
                return;
            _entries.Enqueue(entry);
        }
    }
}
