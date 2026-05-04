using DotNet.Basics.Collections;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys.Text;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNet.Basics.Cli.Logging
{
    public class AnsiConsoleLogger : IConsoleLogger
    {
        private static SysRegex _highlightRegex = @$"{DiagnosticsExtensions.HighlightStart}(?<highlight>.+?){DiagnosticsExtensions.HighlightEnd}";


        public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;

        public void Log(LogLevel level, string message, Exception? e)
        {
            if (level >= MinimumLogLevel)
            {
                GetTexts(level, message).ForEach(AnsiConsole.Write);
                AnsiConsole.Write(Text.NewLine);
                AnsiConsole.Reset();
            }
            if (e != null)
                AnsiConsole.WriteException(e);
        }

        private static IEnumerable<Text> GetTexts(LogLevel level, string msg)
        {
            if (!_highlightRegex.Test(msg))
                yield return new Text(msg, GetStyle(level, msg.IsSuccess()));

            //has highlights
            bool isInHighlight = false;
            var currentText = new StringBuilder();
            foreach (var @char in msg)
            {
                if (@char == DiagnosticsExtensions.HighlightEnd || @char == DiagnosticsExtensions.HighlightStart)
                {
                    var highlighted = currentText.ToString();
                    currentText = new StringBuilder();
                    yield return new Text(highlighted, GetStyle(level, msg.IsSuccess(), isInHighlight));
                    if (isInHighlight && @char == DiagnosticsExtensions.HighlightEnd)
                        isInHighlight = false;
                    else if (!isInHighlight && @char == DiagnosticsExtensions.HighlightStart)
                        isInHighlight = true;
                }
                else
                    currentText.Append(@char);

            }
            yield return new Text(currentText.ToString(), GetStyle(level, msg.IsSuccess(), isInHighlight));
        }


        private static Style GetStyle(LogLevel level, bool isSuccess = false, bool isHighlight = false)
        {
            return isHighlight ? GetHighlightStyle(level, isSuccess) : GetDefaultStyle(level, isSuccess);
        }

        private static Style GetDefaultStyle(LogLevel level, bool isSuccess = false)
        {
            if (isSuccess)
                return new Style(Color.Green, null, Decoration.Dim);

            return level switch
            {
                LogLevel.Trace => new Style(Color.Gray, null, Decoration.Dim),
                LogLevel.Debug => new Style(Color.DarkCyan, null, Decoration.Dim),
                LogLevel.Information => new Style(Color.White, null, Decoration.Dim),
                LogLevel.Warning => new Style(Color.Yellow),
                LogLevel.Error => new Style(Color.Red),
                LogLevel.Critical => new Style(Color.White, Color.DarkRed),
                _ => new Style()
            };
        }

        private static Style GetHighlightStyle(LogLevel level, bool isSuccess = false)
        {
            if (isSuccess)
                return new Style(Color.Green);

            return level switch
            {
                LogLevel.Trace => new Style(Color.Gray),
                LogLevel.Debug => new Style(Color.DarkCyan),
                LogLevel.Information => new Style(Color.Cyan),
                LogLevel.Warning => new Style(Color.Orange1),
                LogLevel.Error => new Style(Color.LightCoral),
                LogLevel.Critical => new Style(Color.White, Color.DarkRed),
                _ => new Style()
            };
        }
    }
}
