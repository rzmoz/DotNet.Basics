using Microsoft.Extensions.Logging;
using DotNet.Basics.Diagnostics;
using System;

namespace DotNet.Basics.Cli.Logging
{
    internal class AdoPipelineLogger : IConsoleLogger
    {
        private static readonly string _successPrefix = "##[section]";
        public LogLevel MinimumLogLevel { get; set; } = LogLevel.Debug;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel lvl) => lvl >= MinimumLogLevel && lvl < LogLevel.None;        

        public void Log<TState>(LogLevel lvl, EventId eventId, TState state, Exception? e, Func<TState, Exception?, string> formatter)
        {
            var msg = formatter.Invoke(state, e);

            if (string.IsNullOrWhiteSpace(msg))
                return;

            var adoPrefix = msg.IsSuccess() ? _successPrefix : GetLogLevelPrefix(lvl);

            if (e != null)
            {
                Console.WriteLine(e.ToString());
                Console.Error.WriteLine($"{GetLogLevelPrefix(LogLevel.Error)}{e.Message}");
            }
            Console.WriteLine($"{adoPrefix}{msg}");
        }

        private string GetLogLevelPrefix(LogLevel level)
        {
            return level switch
            {
                LogLevel.Trace => "##[command]",
                LogLevel.Debug => "##[debug]",
                LogLevel.Information => string.Empty,
                LogLevel.Warning => "##vso[task.logissue type=warning;]",
                LogLevel.Error => "##vso[task.logissue type=error;]",
                LogLevel.Critical => "##vso[task.logissue type=error;]",
                _ => string.Empty
            };
        }
    }
}