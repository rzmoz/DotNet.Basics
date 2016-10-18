using NLog;
using NLog.Conditions;
using NLog.Targets;

namespace DotNet.Basics.NLog
{
    public static class ColoredConsoleTargetExtensions
    {
        public static ColoredConsoleTarget AddDefaultLogColors(this ColoredConsoleTarget target)
        {
            target.AddLogColor(LogLevel.Debug, ConsoleOutputColor.DarkGray)
                  .AddLogColor(LogLevel.Trace, ConsoleOutputColor.DarkCyan)
                  .AddLogColor(LogLevel.Info, ConsoleOutputColor.White)
                  .AddLogColor(LogLevel.Warn, ConsoleOutputColor.Yellow)
                  .AddLogColor(LogLevel.Error, ConsoleOutputColor.Red)
                  .AddLogColor(LogLevel.Fatal, ConsoleOutputColor.White, ConsoleOutputColor.DarkRed);
            return target;
        }

        public static ColoredConsoleTarget AddLogColor(this ColoredConsoleTarget target, LogLevel level, ConsoleOutputColor foregroundColor,
            ConsoleOutputColor backgroundColor = ConsoleOutputColor.NoChange)
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
