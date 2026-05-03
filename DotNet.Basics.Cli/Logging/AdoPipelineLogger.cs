using Microsoft.Extensions.Logging;
using DotNet.Basics.Diagnostics;
using System;

namespace DotNet.Basics.Cli.Logging
{
    internal class AdoPipelineLogger : IConsoleLogger
    {
        private static readonly string _successPrefix = "##[section]";

        public void Log(LogLevel level, string message, Exception? e)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            var adoPrefix = message.IsSuccess() ? _successPrefix : GetLogLevelPrefix(level);

            if (e != null)
            {
                Console.Out.WriteLine(e.ToString());
                Console.Error.WriteLine($"{GetLogLevelPrefix(LogLevel.Error)}{e.Message}");
            }
            Console.Out.WriteLine($"{adoPrefix}{message}");
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