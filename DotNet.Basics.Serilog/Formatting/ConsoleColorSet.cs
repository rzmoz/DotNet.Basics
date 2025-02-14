using System;
using System.Collections;
using System.Collections.Generic;
using Serilog.Events;

namespace DotNet.Basics.Serilog.Formatting
{
    public class ConsoleColorSet : IEnumerable<KeyValuePair<LogEventLevel, ConsoleColor>>
    {
        private readonly IDictionary<LogEventLevel, ConsoleColor> _colors = new Dictionary<LogEventLevel, ConsoleColor>();

        public ConsoleColor this[LogEventLevel lvl] => _colors[lvl];

        public void Add(LogEventLevel lvl, ConsoleColor color)
        {
            _colors.Add(lvl, color);
        }

        public IEnumerator<KeyValuePair<LogEventLevel, ConsoleColor>> GetEnumerator()
        {
            return _colors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
