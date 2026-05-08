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
        private static readonly Func<Progress, Progress> _passThroughProgressFunc = _ => _;
        private static readonly Func<Status, Status> _passThroughStatusFunc = _ => _;
        private static readonly Func<LiveDisplay, LiveDisplay> _passThroughLiveDisplayFunc = _ => _;

        public static string ProgressDefaultMessage { get; set; } = "Starting...";

        public static ConsoleStyleTheme Theme { get; set; } = new DefaultDarkTheme();

        // ------- Progress ------- //
        public static void Progress(this ILogger _, string message, Action<ProgressTask> action)
        {
            Progress(_, message, 0, _passThroughProgressFunc, action);
        }
        public static void Progress(this ILogger _, double maxValue, Action<ProgressTask> action)
        {
            Progress(_, ProgressDefaultMessage, maxValue, _passThroughProgressFunc, action);
        }
        public static void Progress(this ILogger _, string message, double maxValue, Action<ProgressTask> action)
        {
            Progress(_, message, maxValue, _passThroughProgressFunc, action);
        }
        public static void Progress(this ILogger _, string message, Func<Progress, Progress> init, Action<ProgressTask> action)
        {
            Progress(_, message, 0, init, action);
        }
        public static void Progress(this ILogger _, double maxValue, Func<Progress, Progress> init, Action<ProgressTask> action)
        {
            Progress(_, ProgressDefaultMessage, maxValue, init, action);
        }
        public static void Progress(this ILogger _, string message, double maxValue, Func<Progress, Progress> init, Action<ProgressTask> action)
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

        public static async Task ProgressAsync(this ILogger _, string message, Func<ProgressTask, Task> func)
        {
            await ProgressAsync(_, message, 0, _passThroughProgressFunc, func);
        }
        public static async Task ProgressAsync(this ILogger _, double maxValue, Func<ProgressTask, Task> func)
        {
            await ProgressAsync(_, ProgressDefaultMessage, maxValue, _passThroughProgressFunc, func);
        }
        public static async Task ProgressAsync(this ILogger _, string message, double maxValue, Func<ProgressTask, Task> func)
        {
            await ProgressAsync(_, message, maxValue, _passThroughProgressFunc, func);
        }
        public static async Task ProgressAsync(this ILogger _, string message, Func<Progress, Progress> init, Func<ProgressTask, Task> func)
        {
            await ProgressAsync(_, message, 0, init, func);
        }
        public static async Task ProgressAsync(this ILogger _, double maxValue, Func<Progress, Progress> init, Func<ProgressTask, Task> func)
        {
            await ProgressAsync(_, ProgressDefaultMessage, maxValue, init, func);
        }
        public static async Task ProgressAsync(this ILogger _, string message, double maxValue, Func<Progress, Progress> init, Func<ProgressTask, Task> func)
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
        private static Progress InitProgress(Func<Progress, Progress> init)
        {
            var progress = AnsiConsole.Progress().AutoRefresh(true).AutoClear(true);
            progress = init(progress);
            return progress;
        }
        private static (ProgressTask task, CancellationTokenSource cts) InitProgressTask(ProgressContext ctx, string message, double maxValue)
        {
            var task = ctx.AddTask(message, maxValue: maxValue);
            task.IsIndeterminate(maxValue <= 0);
            return (task, new CancellationTokenSource());
        }

        // ------- Status ------- //
        public static void Status(this ILogger _, string startMessage, Action<StatusContext> action)
        {
            Status(_, startMessage, _passThroughStatusFunc, action);
        }
        public static void Status(this ILogger _, string startMessage, Func<Status, Status> init, Action<StatusContext> action)
        {
            InitStatus(startMessage, init).Start(startMessage, action);
        }

        public static async Task StatusAsync(this ILogger _, string startMessage, Func<StatusContext, Task> func)
        {
            await StatusAsync(_, startMessage, _passThroughStatusFunc, func);
        }
        public static async Task StatusAsync(this ILogger _, string startMessage, Func<Status, Status> init, Func<StatusContext, Task> func)
        {
            await InitStatus(startMessage, init).StartAsync(startMessage, func);
        }
        private static Status InitStatus(string startMessage, Func<Status, Status> init)
        {
            var status = AnsiConsole.Status().AutoRefresh(true);
            status = init(status);
            return status;
        }

        // ------- Live ------- //
        public static void Live(this ILogger _, IRenderable renderable, Action<LiveDisplayContext> action)
        {
            Live(_, renderable, _passThroughLiveDisplayFunc, action);
        }
        public static void Live(this ILogger _, IRenderable renderable, Func<LiveDisplay, LiveDisplay> init, Action<LiveDisplayContext> action)
        {
            InitLive(renderable, init).Start(action);
        }

        public static async Task LiveAsync(this ILogger _, IRenderable renderable, Func<LiveDisplayContext, Task> func)
        {
            await LiveAsync(_, renderable, _ => { return _; }, func);
        }
        public static async Task LiveAsync(this ILogger _, IRenderable renderable, Func<LiveDisplay, LiveDisplay> init, Func<LiveDisplayContext, Task> func)
        {
            await InitLive(renderable, init).StartAsync(func);
        }
        private static LiveDisplay InitLive(IRenderable renderable, Func<LiveDisplay, LiveDisplay> init)
        {
            var live = AnsiConsole.Live(renderable).AutoClear(true);
            live = init(live);
            return live;
        }

        // ------- Force Write ------- //
        public static void ForceWriteLineJson(this ILogger log, object o, Func<JsonText, JsonText>? init = null)
        {
            log.ForceWriteLineJson(o.ToJson(), init);
        }
        public static void ForceWriteLineJson(this ILogger log, string str, Func<JsonText, JsonText>? init = null)
        {
            log.ForceWriteLine(init?.Invoke(new JsonText(str)) ?? new JsonText(str));
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
