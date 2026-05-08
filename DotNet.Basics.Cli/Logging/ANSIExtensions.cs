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
        public static string ProgressDefaultMessage { get; set; } = "Starting...";
        public static ConsoleStyleTheme Theme { get; set; } = new DefaultDarkTheme();

        // ------- Progress ------- //
        public static void Progress(this ILogger _, string message, Action<ProgressTask> action, Func<Progress, Progress>? init = null)
        {
            Progress(_, message, 0, action, init);
        }
        public static void Progress(this ILogger _, double maxValue, Action<ProgressTask> action, Func<Progress, Progress>? init = null)
        {
            Progress(_, ProgressDefaultMessage, maxValue, action, init);
        }
        public static void Progress(this ILogger _, string message, double maxValue, Action<ProgressTask> action, Func<Progress, Progress>? init = null)
        {
            InitProgress(init).Start(async ctx =>
            {
                var task = ctx.AddTask(message, maxValue: maxValue);
                task.IsIndeterminate(maxValue <= 0);
                var ct = new CancellationTokenSource();
                while (!ctx.IsFinished && !ct.IsCancellationRequested)
                {
                    action(task);
                    ct.Cancel();
                }
            });
        }
        
        public static async Task ProgressAsync(this ILogger _, string message, Func<ProgressTask, Task> func, Func<Progress, Progress>? init = null)
        {
            await ProgressAsync(_, message, 0, func, init);
        }
        public static async Task ProgressAsync(this ILogger _, double maxValue, Func<ProgressTask, Task> func, Func<Progress, Progress>? init = null)
        {
            await ProgressAsync(_, ProgressDefaultMessage, maxValue, func, init);
        }
        public static async Task ProgressAsync(this ILogger _, string message, double maxValue, Func<ProgressTask, Task> func, Func<Progress, Progress>? init = null)
        {
            await InitProgress(init).StartAsync(async ctx =>
            {
                var p = InitProgressTask(ctx, message, maxValue);
                while (!ctx.IsFinished && !p.cts.IsCancellationRequested)
                {
                    await func(p.task);
                    p.cts.Cancel();
                }
            });
        }
        private static Progress InitProgress(Func<Progress, Progress>? init)
        {
            var progress = AnsiConsole.Progress().AutoRefresh(true).AutoClear(true);
            progress = (init ?? (_ => { return _; })).Invoke(progress);
            return progress;
        }
        private static (ProgressTask task, CancellationTokenSource cts) InitProgressTask(ProgressContext ctx, string message, double maxValue)
        {
            var task = ctx.AddTask(message, maxValue: maxValue);
            task.IsIndeterminate(maxValue <= 0);
            return (task, new CancellationTokenSource());
        }

        // ------- Status ------- //

        public static void Status(this ILogger _, string startMessage, Action<StatusContext> action, Func<Status, Status>? init = null)
        {
            InitStatus(startMessage, init).Start(startMessage, action);
        }

        public static async Task StatusAsync(this ILogger _, string startMessage, Func<StatusContext, Task> func, Func<Status, Status>? init = null)
        {
            await InitStatus(startMessage, init).StartAsync(startMessage, func);
        }
        private static Status InitStatus(string startMessage, Func<Status, Status>? init)
        {
            var status = AnsiConsole.Status().AutoRefresh(true);
            status = (init ?? (_ => { return _; })).Invoke(status);
            return status;
        }

        // ------- Live ------- //

        public static void Live(this ILogger _, IRenderable renderable, Action<LiveDisplayContext> action, Func<LiveDisplay, LiveDisplay>? init = null)
        {
            InitLive(renderable, init).Start(action);
        }
        public static async Task LiveAsync(this ILogger _, IRenderable renderable, Func<LiveDisplayContext, Task> func, Func<LiveDisplay, LiveDisplay>? init = null)
        {
            await InitLive(renderable, init).StartAsync(func);
        }
        private static LiveDisplay InitLive(IRenderable renderable, Func<LiveDisplay, LiveDisplay>? init)
        {
            var live = AnsiConsole.Live(renderable).AutoClear(true);
            live = (init ?? (_ => { return _; })).Invoke(live);
            return live;
        }

        // ------- Force Write ------- //
        public static ILogger ForceWriteLineJson(this ILogger _, object o, Func<JsonText, JsonText>? init = null)
        {
            return ForceWriteLineJson(_, o.ToJson(), init);
        }
        public static ILogger ForceWriteLineJson(this ILogger _, string str, Func<JsonText, JsonText>? init = null)
        {
            return ForceWriteLine(_, init?.Invoke(new JsonText(str)) ?? new JsonText(str));
        }

        /// <summary>
        /// Outputs to console regardless of loglevel but in chosen loglevel color scheme
        /// </summary>
        public static ILogger ForceWriteLine(this ILogger _, string str, LogLevel lvl = LogLevel.Debug, bool isSuccess = false, bool isHighlight = false) => ForceWriteLine(_, str, Theme.GetStyle(lvl, isSuccess, isHighlight));
        /// <summary>
        /// Outputs to console regardless of loglevel
        /// </summary>
        public static ILogger ForceWriteLine(this ILogger _, string str, Style? style) => ForceWriteLine(_, new Text(str, style));
        /// <summary>
        /// Outputs to console regardless of loglevel
        /// </summary>
        public static ILogger ForceWriteLine(this ILogger _, params IEnumerable<(string str, Style? style)> strs) => ForceWriteLine(_, strs.Select(str => new Text(str.str, str.style)));
        /// <summary>
        /// Outputs to console regardless of loglevel
        /// </summary>
        public static ILogger ForceWriteLine(this ILogger _, params IEnumerable<IRenderable> renderables)
        {
            ForceWrite(_, renderables);
            ForceWrite(_, Text.NewLine);
            return _;
        }

        /// <summary>
        /// Outputs to console regardless of loglevel but in chosen loglevel color scheme
        /// </summary>
        public static ILogger ForceWrite(this ILogger _, string str, LogLevel lvl = LogLevel.Debug, bool isSuccess = false, bool isHighlight = false) => ForceWrite(_, str, Theme.GetStyle(lvl, isSuccess, isHighlight));
        /// <summary>
        /// Outputs to console regardless of loglevel
        /// </summary>
        public static ILogger ForceWrite(this ILogger _, string str, Style? style) => ForceWrite(_, new Text(str, style));
        /// <summary>
        /// Outputs to console regardless of loglevel
        /// </summary>
        public static ILogger ForceWrite(this ILogger _, params IEnumerable<(string str, Style? style)> strs) => ForceWrite(_, strs.Select(s => new Text(s.str, s.style)));
        /// <summary>
        /// Outputs to console regardless of loglevel
        /// </summary>
        public static ILogger ForceWrite(this ILogger _, params IEnumerable<IRenderable> renderables)
        {
            renderables.ForEach(AnsiConsole.Write);
            return _;
        }
    }
}
