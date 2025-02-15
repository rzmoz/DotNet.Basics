using System;
using System.Collections;
using System.Collections.Generic;
using Serilog.Events;

namespace DotNet.Basics.Serilog.Formatting
{
    public class ConsoleColorSet(ConsoleColor foreground, ConsoleColor? background = null) : IEnumerable<KeyValuePair<LogEventLevel, (ConsoleColor Foreground, ConsoleColor Background)>>
    {
        private readonly Dictionary<LogEventLevel, (ConsoleColor Forground, ConsoleColor Background)> _colors = new();

        public (ConsoleColor Foreground, ConsoleColor Background) Success { get; } = (foreground, background ?? ConsoleColor.Black);
        public (ConsoleColor Foreground, ConsoleColor Background) this[LogEventLevel lvl] => _colors[lvl];

        public void Add(LogEventLevel lvl, ConsoleColor foreground, ConsoleColor? background = null)
        {
            _colors.Add(lvl, (foreground, background ?? ConsoleColor.Black));
        }

        public IEnumerator<KeyValuePair<LogEventLevel, (ConsoleColor Foreground, ConsoleColor Background)>> GetEnumerator()
        {
            return _colors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
