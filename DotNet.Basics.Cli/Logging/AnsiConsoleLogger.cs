using DotNet.Basics.Diagnostics;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using System;

namespace DotNet.Basics.Cli.Logging
{
    public class AnsiConsoleLogger : IConsoleLogger
    {
        public void Log(LogLevel level, string message, Exception? e)
        {
            AnsiConsole.Write(GetText(level, message));
            AnsiConsole.Write(Text.NewLine);
            if (e != null)
                AnsiConsole.WriteException(e);
        }
        private static Text GetText(LogLevel level, string msg)
        {
            if (msg.IsSuccess())
                return new Text(msg, new Style(Color.DarkGreen));
            return new Text(msg, GetStyle(level));
        }
        private static Style GetStyle(LogLevel level)
        {
            return level switch
            {
                LogLevel.Trace => new Style(Color.Gray),
                LogLevel.Debug => new Style(Color.DarkViolet),
                LogLevel.Information => new Style(Color.Cyan),
                LogLevel.Warning => new Style(Color.Yellow),
                LogLevel.Error => new Style(Color.Red),
                LogLevel.Critical => new Style(Color.White, Color.DarkRed),
                _ => new Style()
            };
        }
    }
}
