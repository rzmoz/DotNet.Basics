using DotNet.Basics.Collections;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;
using DotNet.Basics.Sys.Text;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace DotNet.Basics.Cli.Logging
{
    public class DevConsole : ILogger, IConsoleLogger
    {
        [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        const int VK_RETURN = 0x0D;
        const int WM_KEYDOWN = 0x100;

        // ------- ILogger section ------- //
        public bool IsEnabled(LogLevel lvl) => lvl >= MinimumLogLevel && lvl < LogLevel.None;
        public void Log<TState>(LogLevel lvl, EventId eventId, TState state, Exception? e, Func<TState, Exception?, string> formatter) => Log(lvl, formatter.Invoke(state, e), e);
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        // ------- Dotnet.Basics section ------- //
        private static SysRegex _highlightRegex = @$"{DiagnosticsExtensions.HighlightStart}(?<highlight>.+?){DiagnosticsExtensions.HighlightEnd}";
        public static readonly DevConsole Console = new DevConsole { MinimumLogLevel = LogLevel.Trace };
        public static void PauseForDebuggerAttach()
        {
            PauseForDebuggerAttach(30.Seconds());
        }

        public static void PauseForDebuggerAttach(TimeSpan timeout)
        {
            AnsiConsole.Markup($"Pausing to attach debugger. [orange1]{Process.GetCurrentProcess().MainModule?.FileName.ToFile().Name ?? "???"}[/] <{Environment.ProcessId}>. [yellow]Press enter to continue[/]");
            // Start Console.ReadLine asynchronously
            Task<string> inputTask = Task.Factory.StartNew(() => System.Console.ReadLine() ?? string.Empty);
            Task.WaitAny(new Task[] { inputTask }, timeout);
        }

        public ConsoleStyleTheme Theme { get; set; } = new DefaultDarkTheme();
        public LogLevel MinimumLogLevel { get; set; }

        public void Log(LogLevel level, string message, Exception? e)
        {
            if (e != null)
                AnsiConsole.WriteException(e);

            if (level >= MinimumLogLevel && level < LogLevel.None)
            {
                var texts = GetTexts(level, message).ToList();
                texts.ForEach(AnsiConsole.Write);
                AnsiConsole.Write(Text.NewLine);
                AnsiConsole.Reset();
            }
        }

        // ------- Spectre.Console ------- //
        public async Task ProgressAsync(string message, double maxValue, Func<ProgressTask, Task> func)
        {
            await ProgressAsync(message, maxValue, _ => { return _; }, func);
        }
        public async Task ProgressAsync(string message, double maxValue, Func<Progress, Progress> init, Func<ProgressTask, Task> func)
        {
            var progress = AnsiConsole.Progress().AutoRefresh(true).AutoClear(true);
            progress = init(progress);
            await progress.StartAsync(async ctx =>
            {
                var task = ctx.AddTask(message, maxValue: maxValue);
                var ct = new CancellationTokenSource();
                while (!ctx.IsFinished && !ct.IsCancellationRequested)
                {
                    await func(task);
                    ct.Cancel();
                }
            });
        }
        public async Task StatusAsync(string startMessage, Func<StatusContext, Task> func)
        {
            await StatusAsync(startMessage, _ => { return _; }, func);
        }
        public async Task StatusAsync(string startMessage, Func<Status, Status> init, Func<StatusContext, Task> func)
        {
            var status = AnsiConsole.Status().AutoRefresh(true);
            status = init(status);
            await status.StartAsync(startMessage, func);
        }

        public async Task LiveAsync(IRenderable renderable, Func<LiveDisplayContext, Task> func)
        {
            await LiveAsync(renderable, _ => { return _; }, func);
        }
        public async Task LiveAsync(IRenderable renderable, Func<LiveDisplay, LiveDisplay> init, Func<LiveDisplayContext, Task> func)
        {
            var live = AnsiConsole.Live(renderable).AutoClear(true);
            live = init(live);
            await live.StartAsync(func);
        }

        public void LogJson(string str, LogLevel lvl = LogLevel.Debug) => ForceWriteLine(str, Theme.GetStyle(lvl));

        /// <summary>
        /// Outputs to console regardless of loglevel but in chosen loglevel color scheme
        /// </summary>
        public void ForceWriteLine(string str, LogLevel lvl = LogLevel.Debug, bool isSuccess = false, bool isHighlight = false) => ForceWriteLine(str, Theme.GetStyle(lvl, isSuccess, isHighlight));
        /// <summary>
        /// Outputs to console regardless of loglevel
        /// </summary>
        public void ForceWriteLine(string str, Style? style) => ForceWriteLine(new Text(str, style));
        /// <summary>
        /// Outputs to console regardless of loglevel
        /// </summary>
        public void ForceWriteLine(params IEnumerable<(string str, Style? style)> strs) => ForceWriteLine(strs.Select(str => new Text(str.str, str.style)));
        /// <summary>
        /// Outputs to console regardless of loglevel
        /// </summary>
        public void ForceWriteLine(params IEnumerable<IRenderable> renderables) => renderables.Append(Text.NewLine).ForEach(AnsiConsole.Write);

        /// <summary>
        /// Outputs to console regardless of loglevel but in chosen loglevel color scheme
        /// </summary>
        public void ForceWrite(string str, LogLevel lvl = LogLevel.Debug, bool isSuccess = false, bool isHighlight = false) => ForceWrite(str, Theme.GetStyle(lvl, isSuccess, isHighlight));
        /// <summary>
        /// Outputs to console regardless of loglevel
        /// </summary>
        public void ForceWrite(string str, Style? style) => ForceWrite(new Text(str, style));
        /// <summary>
        /// Outputs to console regardless of loglevel
        /// </summary>
        public void ForceWrite(params IEnumerable<(string str, Style? style)> strs) => ForceWrite(strs.Select(s => new Text(s.str, s.style)));
        /// <summary>
        /// Outputs to console regardless of loglevel
        /// </summary>
        public void ForceWrite(params IEnumerable<IRenderable> renderables) => renderables.ForEach(AnsiConsole.Write);



        private IReadOnlyList<Text> GetTexts(LogLevel level, string msg)
        {
            if (!_highlightRegex.Test(msg))
                return [new Text(msg, Theme.GetStyle(level, msg.IsSuccess()))];
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