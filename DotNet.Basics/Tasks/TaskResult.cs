using System;
using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Diagnostics;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Tasks
{
    public class TaskResult
    {
        public TaskResult()
            : this(null, new List<LogEntry>())
        { }
        public TaskResult(string taskName)
            : this(taskName, new List<LogEntry>())
        { }

        public TaskResult(Action<ConcurrentLog> addLogEntries)
            : this(null, ConstructList(addLogEntries))
        { }

        public TaskResult(string taskName, Action<ConcurrentLog> addLogEntries)
            : this(taskName, ConstructList(addLogEntries))
        { }

        public TaskResult(string taskName, IEnumerable<LogEntry> logEntries)
        {
            Name = taskName ?? string.Empty;
            Log = logEntries?.ToList() ?? new List<LogEntry>();
        }

        public string Name { get; }
        public IReadOnlyCollection<LogEntry> Log { get; }

        public TaskResult Append(Action<ConcurrentLog> addLogEntries)
        {
            return new TaskResult(Name, log =>
            {
                log.AddRange(Log);
                addLogEntries?.Invoke(log);
            });
        }

        private static ConcurrentLog ConstructList(Action<ConcurrentLog> addLogEntries)
        {
            var log = new ConcurrentLog();
            addLogEntries?.Invoke(log);
            return log;
        }
    }
}
