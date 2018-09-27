using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Diagnostics
{
    public class ConcurrentLog : IReadOnlyCollection<LogEntry>
    {
        private readonly ConcurrentQueue<LogEntry> _entries;

        public ConcurrentLog()
        {
            _entries = new ConcurrentQueue<LogEntry>();
        }

        public ConcurrentLog(IEnumerable<LogEntry> entries)
        {
            _entries = new ConcurrentQueue<LogEntry>(entries);
        }

        public void AddRange(IEnumerable<LogEntry> entries)
        {
            foreach (var entry in entries)
                _entries.Enqueue(entry);
        }

        public void Add(LogLevel logLevel, string message)
        {
            _entries.Enqueue(new LogEntry(logLevel, message));
        }

        public void Add(LogLevel logLevel, Exception e)
        {
            _entries.Enqueue(new LogEntry(logLevel, e.Message, e));
        }

        public void Add(LogLevel logLevel, string message, Exception e)
        {
            _entries.Enqueue(new LogEntry(logLevel, message, e));
        }

        public void Add(LogEntry entry)
        {
            _entries.Enqueue(entry);
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
