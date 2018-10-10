using NLog;
using NLog.Conditions;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace DotNet.Basics.NLog
{
    public static class NLogTargetExtensions
    {
        /// <summary>
        /// https://github.com/nlog/NLog/wiki/BufferingWrapper-target
        /// </summary>
        /// <param name="wrappedTarget"></param>
        /// <param name="bufferSize"></param>
        /// <param name="flushTimeout"></param>
        /// <returns></returns>
        public static BufferingTargetWrapper AsBuffered(this Target wrappedTarget, int bufferSize = 100, int flushTimeout = 1000)
        {
            return new BufferingTargetWrapper(wrappedTarget, bufferSize, flushTimeout);
        }

        /// <summary>
        /// https://github.com/nlog/NLog/wiki/AsyncWrapper-target
        /// </summary>
        /// <param name="wrappedTarget"></param>
        /// <param name="queueLimit"></param>
        /// <param name="overflowAction"></param>
        /// <returns></returns>
        public static AsyncTargetWrapper AsAsync(this Target wrappedTarget, int queueLimit = 10000, AsyncTargetWrapperOverflowAction overflowAction = AsyncTargetWrapperOverflowAction.Grow)
        {
            return new AsyncTargetWrapper(wrappedTarget, queueLimit, overflowAction);
        }

        public static ColoredConsoleTarget WithDefaultColors(this ColoredConsoleTarget target)
        {
            target.RowHighlightingRules.Clear();
            target.AddLogColor(LogLevel.Trace, ConsoleOutputColor.Gray)
                .AddLogColor(LogLevel.Debug, ConsoleOutputColor.DarkGray)
                .AddLogColor(LogLevel.Info, ConsoleOutputColor.White)
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
