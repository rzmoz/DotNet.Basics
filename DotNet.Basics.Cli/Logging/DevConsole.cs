using DotNet.Basics.Collections;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;
using DotNet.Basics.Sys.Text;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Basics.Cli.Logging
{
    public class DevConsole : IConsoleLogger
    {
        private static SysRegex _highlightRegex = @$"{DiagnosticsExtensions.HighlightStart}(?<highlight>.+?){DiagnosticsExtensions.HighlightEnd}";

        // ------- ILogger section ------- //        
        public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel lvl) => lvl >= MinimumLogLevel && lvl < LogLevel.None;
        public virtual void Log<TState>(LogLevel lvl, EventId eventId, TState state, Exception? e, Func<TState, Exception?, string> formatter)
        {
            var msg = formatter.Invoke(state, e);

            if (e != null)
                AnsiConsole.WriteException(e);

            if (lvl >= MinimumLogLevel && lvl < LogLevel.None)
            {
                GetTexts(lvl, msg).ForEach(AnsiConsole.Write);
                AnsiConsole.Write(Text.NewLine);
                AnsiConsole.Reset();
            }
        }


        // ------- Dotnet.Basics section ------- //        
        public static ILogger Console { get; } = new DevConsole();
        public static TimeSpan PauseForDebuggerAttachTimeout { get; set; } = 30.Seconds();
        public static void PauseForDebuggerAttach() => PauseForDebuggerAttach(PauseForDebuggerAttachTimeout);

        public static void PauseForDebuggerAttach(TimeSpan timeout)
        {
            AnsiConsole.MarkupLine($@"[white dim]Pausing to attach debugger[/] [white]{DiagnosticsExtensions.HighlightStart}[/] [orange1]{Process.GetCurrentProcess().MainModule?.FileName.ToFile().Name ?? "???"}[/][white]:[/][orange1]{Environment.ProcessId}[/]");
            AnsiConsole.Markup($"[yellow]Press enter to continue[/]");
            // Start Console.ReadLine asynchronously
            Task<string> inputTask = Task.Factory.StartNew(() => System.Console.ReadLine() ?? string.Empty);
            Task.WaitAny(new Task[] { inputTask }, timeout);
        }

        private IReadOnlyList<Text> GetTexts(LogLevel level, string msg)
        {
            if (!_highlightRegex.Test(msg))
                return [new Text(msg, ANSIExtensions.Theme.GetStyle(level, msg.IsSuccess()))];
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
                        texts.Add(new Text(highlighted, ANSIExtensions.Theme.GetStyle(level, msg.IsSuccess(), isInHighlight)));
                        if (isInHighlight && @char == DiagnosticsExtensions.HighlightEnd)
                            isInHighlight = false;
                        else if (!isInHighlight && @char == DiagnosticsExtensions.HighlightStart)
                            isInHighlight = true;
                    }
                    else
                        currentText.Append(@char);
                }
                texts.Add(new Text(currentText.ToString(), ANSIExtensions.Theme.GetStyle(level, msg.IsSuccess(), isInHighlight)));
                return texts;
            }
        }
    }
}