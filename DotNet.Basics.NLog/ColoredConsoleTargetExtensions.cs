﻿using NLog;
using NLog.Conditions;
using NLog.Targets;

namespace DotNet.Basics.NLog
{
    public static class ColoredConsoleTargetExtensions
    {
        public static ColoredConsoleTarget WithDefaultColors(this ColoredConsoleTarget target)
        {
            target.RowHighlightingRules.Clear();
            target.AddLogColor(LogLevel.Debug, ConsoleOutputColor.DarkGray)
                  .AddLogColor(LogLevel.Trace, ConsoleOutputColor.Gray)
                  .AddLogColor(LogLevel.Info, ConsoleOutputColor.Cyan)
                  .AddLogColor(LogLevel.Warn, ConsoleOutputColor.Yellow, ConsoleOutputColor.Black)
                  .AddLogColor(LogLevel.Error, ConsoleOutputColor.Red, ConsoleOutputColor.Black)
                  .AddLogColor(LogLevel.Fatal, ConsoleOutputColor.White, ConsoleOutputColor.DarkRed);
            return target;
        }

        public static ColoredConsoleTarget AddLogColor(this ColoredConsoleTarget target, LogLevel level, ConsoleOutputColor foregroundColor, ConsoleOutputColor backgroundColor = ConsoleOutputColor.NoChange)
        {
            target.RowHighlightingRules.Add(new ConsoleRowHighlightingRule
            {
                Condition = ConditionParser.ParseExpression($"level == LogLevel.{level}"),
                BackgroundColor = backgroundColor,
                ForegroundColor = foregroundColor
            });
            return target;
        }
    }
}
