using DotNet.Basics.Collections;
using DotNet.Basics.Sys.Text;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Json;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Cli.Logging
{
    // ------- Spectre.Console ------- //
    public static class ANSIExtensions
    {
        public static ConsoleStyleTheme Theme { get; set; } = new DefaultDarkTheme();

        public static async Task ProgressAsync(this ILogger log, string message, double maxValue, Func<ProgressTask, Task> func)
        {
            await log.ProgressAsync(message, maxValue, _ => { return _; }, func);
        }
        public static async Task ProgressAsync(this ILogger log, string message, double maxValue, Func<Progress, Progress> init, Func<ProgressTask, Task> func)
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
        public static async Task StatusAsync(this ILogger log, string startMessage, Func<StatusContext, Task> func)
        {
            await log.StatusAsync(startMessage, _ => { return _; }, func);
        }
        public static async Task StatusAsync(this ILogger log, string startMessage, Func<Status, Status> init, Func<StatusContext, Task> func)
        {
            var status = AnsiConsole.Status().AutoRefresh(true);
            status = init(status);
            await status.StartAsync(startMessage, func);
        }

        public static async Task LiveAsync(this ILogger log, IRenderable renderable, Func<LiveDisplayContext, Task> func)
        {
            await log.LiveAsync(renderable, _ => { return _; }, func);
        }
        public static async Task LiveAsync(this ILogger log, IRenderable renderable, Func<LiveDisplay, LiveDisplay> init, Func<LiveDisplayContext, Task> func)
        {
            var live = AnsiConsole.Live(renderable).AutoClear(true);
            live = init(live);
            await live.StartAsync(func);
        }

        public static void ForceWriteJson(this ILogger log, object o, Func<JsonText, JsonText>? init = null)
        {
            log.ForceWriteJson(o.ToJson(), init);
        }
        public static void ForceWriteJson(this ILogger log, string str, Func<JsonText, JsonText>? init = null)
        {
            log.ForceWrite(init?.Invoke(new JsonText(str)) ?? new JsonText(str));
        }

        /// <summary>
        /// Outputs to console regardless of loglevel but in chosen loglevel color scheme
        /// </summary>
        public static void ForceWriteLine(this ILogger log, string str, LogLevel lvl = LogLevel.Debug, bool isSuccess = false, bool isHighlight = false) => log.ForceWriteLine(str, Theme.GetStyle(lvl, isSuccess, isHighlight));
        /// <summary>
        /// Outputs to console regardless of loglevel
        /// </summary>
        public static void ForceWriteLine(this ILogger log, string str, Style? style) => log.ForceWriteLine(new Text(str, style));
        /// <summary>
        /// Outputs to console regardless of loglevel
        /// </summary>
        public static void ForceWriteLine(this ILogger log, params IEnumerable<(string str, Style? style)> strs) => log.ForceWriteLine(strs.Select(str => new Text(str.str, str.style)));
        /// <summary>
        /// Outputs to console regardless of loglevel
        /// </summary>
        public static void ForceWriteLine(this ILogger log, params IEnumerable<IRenderable> renderables) => renderables.Append(Text.NewLine).ForEach(AnsiConsole.Write);

        /// <summary>
        /// Outputs to console regardless of loglevel but in chosen loglevel color scheme
        /// </summary>
        public static void ForceWrite(this ILogger log, string str, LogLevel lvl = LogLevel.Debug, bool isSuccess = false, bool isHighlight = false) => log.ForceWrite(str, Theme.GetStyle(lvl, isSuccess, isHighlight));
        /// <summary>
        /// Outputs to console regardless of loglevel
        /// </summary>
        public static void ForceWrite(this ILogger log, string str, Style? style) => log.ForceWrite(new Text(str, style));
        /// <summary>
        /// Outputs to console regardless of loglevel
        /// </summary>
        public static void ForceWrite(this ILogger log, params IEnumerable<(string str, Style? style)> strs) => log.ForceWrite(strs.Select(s => new Text(s.str, s.style)));
        /// <summary>
        /// Outputs to console regardless of loglevel
        /// </summary>
        public static void ForceWrite(this ILogger log, params IEnumerable<IRenderable> renderables) => renderables.ForEach(AnsiConsole.Write);
    }
}
