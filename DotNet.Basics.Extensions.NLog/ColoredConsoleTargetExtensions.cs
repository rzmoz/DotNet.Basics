using NLog;
using NLog.Conditions;
using NLog.Targets;

namespace DotNet.Basics.Extensions.NLog
{
    public static class ColoredConsoleTargetExtensions
    {
        public static ColoredConsoleTarget WithOutputColors(this ColoredConsoleTarget target,
            ConsoleOutputColor debugColor = ConsoleOutputColor.DarkGray,
            ConsoleOutputColor traceColor = ConsoleOutputColor.Cyan,
            ConsoleOutputColor infoColor = ConsoleOutputColor.White,
            ConsoleOutputColor warnColor = ConsoleOutputColor.Yellow,
            ConsoleOutputColor errorColor = ConsoleOutputColor.Red,
            ConsoleOutputColor fatalForeColor = ConsoleOutputColor.White,
            ConsoleOutputColor fatalBackColor = ConsoleOutputColor.DarkRed)
        {
            target.RowHighlightingRules.Clear();
            target.AddLogColor(LogLevel.Debug, debugColor)
                  .AddLogColor(LogLevel.Trace, traceColor)
                  .AddLogColor(LogLevel.Info, infoColor)
                  .AddLogColor(LogLevel.Warn, warnColor)
                  .AddLogColor(LogLevel.Error, errorColor)
                  .AddLogColor(LogLevel.Fatal, fatalForeColor, fatalBackColor);
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
