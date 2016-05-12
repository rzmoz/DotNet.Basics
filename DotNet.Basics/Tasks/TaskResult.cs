using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Diagnostics;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Tasks
{
    public class TaskResult : IReadOnlyCollection<LogEntry>
    {
        private readonly IReadOnlyCollection<LogEntry> _entries;

        public TaskResult(bool finished = true, string name = null)
            : this(Enumerable.Empty<LogEntry>(), finished, name)
        {

        }
        public TaskResult(IEnumerable<LogEntry> entries, bool finished = true, string name = null)
        {
            if (entries == null) throw new ArgumentNullException(nameof(entries));
            _entries = entries.ToList();
            Failed = _entries.Any(e => e.Level == LogLevel.Error || e.Level == LogLevel.Critical);
            Finished = finished;
            Name = name ?? string.Empty;
        }

        public string Name { get; }
        public bool Finished { get; }
        public bool Failed { get; }

        public IReadOnlyCollection<LogEntry> Debugs => GetLogs(LogLevel.Debug);
        public IReadOnlyCollection<LogEntry> Verboses => GetLogs(LogLevel.Verbose);
        public IReadOnlyCollection<LogEntry> Informations => GetLogs(LogLevel.Information);
        public IReadOnlyCollection<LogEntry> Warnings => GetLogs(LogLevel.Warning);
        public IReadOnlyCollection<LogEntry> Errors => GetLogs(LogLevel.Error);
        public IReadOnlyCollection<LogEntry> Criticals => GetLogs(LogLevel.Critical);

        public IReadOnlyCollection<T> Get<T>() where T : LogEntry
        {
            return _entries.OfType<T>().ToList();
        }

        public IReadOnlyCollection<LogEntry> GetLogs(LogLevel logLevel)
        {
            return _entries.Where(entry => entry.Level == logLevel).ToList();
        }

        public IEnumerator<LogEntry> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _entries.Count;
    }
}
