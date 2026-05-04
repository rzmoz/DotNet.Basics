using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys.Text;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNet.Basics.Cli.Logging
{
    public class AnsiConsoleLogger(ConsoleStyleTheme? theme = null) : IConsoleLogger
    {
        private static SysRegex _highlightRegex = @$"{DiagnosticsExtensions.HighlightStart}(?<highlight>.+?){DiagnosticsExtensions.HighlightEnd}";

        public ConsoleStyleTheme Theme { get; set; } = theme ?? new DefaultDarkTheme();
        public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;

        public void Log(LogLevel level, string message, Exception? e)
        {
            if (level >= MinimumLogLevel && level < LogLevel.None)
            {
                var texts = GetTexts(level, message).ToList();
                texts.ForEach(AnsiConsole.Write);
                AnsiConsole.Write(Text.NewLine);
                AnsiConsole.Reset();
            }
            if (e != null)
                AnsiConsole.WriteException(e);
        }

        private IReadOnlyList<Text> GetTexts(LogLevel level, string msg)
        {
            if (!_highlightRegex.Test(msg))
            {
                return [new Text(msg, Theme.GetStyle(level, msg.IsSuccess()))];
            }
            else
            {
                //has highlights
                bool isInHighlight = false;
                var currentText = new StringBuilder();
                var texts = new List<Text>();

                foreach (var @char in msg)
                {
                    if (@char == DiagnosticsExtensions.HighlightEnd || @char == DiagnosticsExtensions.HighlightStart)
                    {
                        var highlighted = currentText.ToString();
                        currentText = new StringBuilder();
                        texts.Add(new Text(highlighted, Theme.GetStyle(level, msg.IsSuccess(), isInHighlight)));
                        if (isInHighlight && @char == DiagnosticsExtensions.HighlightEnd)
                            isInHighlight = false;
                        else if (!isInHighlight && @char == DiagnosticsExtensions.HighlightStart)
                            isInHighlight = true;
                    }
                    else
                        currentText.Append(@char);
                }
                texts.Add(new Text(currentText.ToString(), Theme.GetStyle(level, msg.IsSuccess(), isInHighlight)));
                return texts;
            }
        }
    }
}
