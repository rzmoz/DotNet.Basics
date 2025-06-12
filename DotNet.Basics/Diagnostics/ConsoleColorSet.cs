using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DotNet.Basics.Diagnostics
{
    public class ConsoleColorSet(ConsoleColor foreground, ConsoleColor? background = null) : IEnumerable<KeyValuePair<LogLevel, (ConsoleColor Foreground, ConsoleColor Background)>>
    {
        private readonly Dictionary<LogLevel, (ConsoleColor Forground, ConsoleColor Background)> _colors = new();

        public (ConsoleColor Foreground, ConsoleColor Background) Success { get; } = (foreground, background ?? ConsoleColor.Black);
        public (ConsoleColor Foreground, ConsoleColor Background) this[LogLevel lvl] => _colors[lvl];

        public void Add(LogLevel lvl, ConsoleColor foreground, ConsoleColor? background = null)
        {
            _colors.Add(lvl, (foreground, background ?? ConsoleColor.Black));
        }

        public IEnumerator<KeyValuePair<LogLevel, (ConsoleColor Foreground, ConsoleColor Background)>> GetEnumerator()
        {
            return _colors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
