using DotNet.Basics.Collections;
using DotNet.Basics.Sys;
using DotNet.Basics.Sys.Text;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Json;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            InitStatus(init).Start(startMessage, action);
        }

        public static async Task StatusRandomMessagesAsync(this ILogger _, Func<StatusContext, Task> func, IReadOnlyList<string>? randomMessages = null)
        {
            randomMessages ??= _defaultRandomWaitingMessages;

            await InitStatus(status => status
            .Spinner(GetRandom(_spinners))
            .SpinnerStyle(Style.Parse(_rainbowColors[0])))
            .StartAsync($"{GetRandom(randomMessages)}...", async ctx =>
            {
                var cts = new CancellationTokenSource();
                var hexColor = GetRandom(_rainbowColors);
                var message = GetRandom(randomMessages);
                await Task.WhenAny(
                    Task.Run(async () =>
                    {
                        var colorIndex = 0;
                        while (true)
                        {
                            hexColor = _rainbowColors[(++colorIndex) % _rainbowColors.Count];
                            ctx.SpinnerStyle(Style.Parse(hexColor));
                            ctx.Status($"[{hexColor}]{message}...[/]");
                            await Task.Delay(20.MilliSeconds());
                        }

                    }),
                    Task.Run(async () =>
                    {
                        while (true)
                        {
                            message = GetRandom(randomMessages);
                            ctx.Spinner(GetRandom(_spinners));
                            await Task.Delay(3.Seconds());
                        }
                    }),
                    Task.Run(async () =>
                    {
                        await func(ctx);
                        cts.Cancel();
                    }));
            });
        }
        private static T GetRandom<T>(IReadOnlyList<T> list)
        {
            return list[new Random().Next(0, list.Count)];
        }

        public static async Task StatusAsync(this ILogger _, string startMessage, Func<StatusContext, Task> func, Func<Status, Status>? init = null)
        {
            await InitStatus(init).StartAsync(startMessage, func);
        }
        private static Status InitStatus(Func<Status, Status>? init)
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
        private static readonly IReadOnlyList<string> _defaultRandomWaitingMessages = [
  "Please wait… a committee of confused knights is debating your request.",
  "Hold on… a duck is being weighed for scientific reasons no one understands.",
  "One moment… the Ministry of Silly Walks is adjusting its gait.",
  "Loading… a man with three buttocks is reviewing the paperwork.",
  "Please wait… a parrot is being inspected for signs of life.",
  "Hold tight… a shrubbery has been ordered and must be admired.",
  "One moment… the Knights Who Say Ni are practicing new syllables.",
  "Loading… a peasant is arguing about political systems again.",
  "Please wait… someone is shouting about spam in the background.",
  "Hold on… a giant foot is preparing to descend dramatically.",
  "Loading… the Spanish Inquisition is running fashionably late.",
  "Please wait… a coconut-based horse transport system is being calibrated.",
  "One moment… a man in a bowler hat is narrating unnecessarily.",
  "Hold tight… a fish-slapping dance is in progress.",
  "Loading… a wizard is checking if this counts as magic or paperwork.",
  "Please wait… a penguin on the telly is warming up to explode.",
  "One moment… a cheese shop with no cheese is being audited.",
  "Hold on… a lumberjack is adjusting his suspenders.",
  "Loading… a philosopher is arguing with a goat.",
  "Please wait… someone is teaching self-defense using fruit.",
  "One moment… a Viking choir is rehearsing loudly.",
  "Hold tight… a man insists this message is something completely different.",
  "Loading… a dead collector is sorting out who is or isn’t dead yet.",
  "Please wait… a silly argument is escalating into a sillier argument.",
  "One moment… a knight is polishing his coconuts.",
  "Hold on… a bureaucrat is stamping things for no reason.",
  "Loading… a man is returning a parrot that is definitely not alive.",
  "Please wait… a choir of accountants is warming up.",
  "One moment… a giant cartoon hand is pointing dramatically.",
  "Hold tight… a philosopher is searching for his misplaced beard.",
  "Loading… a man insists he’s not the Messiah, just a naughty boy.",
  "Please wait… a sheep is being interviewed for a documentary.",
  "One moment… French taunters are preparing fresh insults.",
  "Hold on… a man is selling arguments by the minute.",
  "Loading… a historian is narrating events that never happened.",
  "Please wait… a knight is bravely running away.",
  "One moment… monks are chanting and hitting themselves with boards.",
  "Hold tight… a castle full of rabbits is being secured.",
  "Loading… a man is attempting a very silly walk.",
  "Please wait… a witch trial is being conducted using vegetables.",
  "One moment… a man is trying to explain the airspeed of an unladen swallow.",
  "Hold on… a group of knights is lost in a forest of moderate inconvenience.",
  "Loading… a man is shouting “Get on with it!” repeatedly.",
  "Please wait… a cow is being catapulted for testing purposes.",
  "One moment… a minstrel is composing a ballad about your patience.",
  "Hold tight… a man is denying he said “Ni” at any point.",
  "Loading… a castle guard is practicing increasingly silly taunts.",
  "Please wait… a man is attempting to open a door that isn’t locked.",
  "One moment… a confused king is asking peasants about mud-based government.",
  "Hold on… a wizard is misplacing his staff again.",
  "Loading… a knight is stuck in a shrubbery-related dilemma.",
  "Please wait… a man is insisting he didn’t vote for the king.",
  "One moment… a group of philosophers is debating the meaning of cheese.",
  "Hold tight… a man is trying to return a cheese that never existed.",
  "Loading… a knight is bravely negotiating with a rabbit.",
  "Please wait… a man is practicing his silly voice for official duties.",
  "One moment… a Viking is chanting about spam again.",
  "Hold on… a man is attempting to walk normally but failing.",
  "Loading… a group of monks is rehearsing their self‑bonking technique.",
  "Please wait… a man is trying to determine if something is, in fact, a witch.",
  "One moment… a knight is stuck saying “Ekki-ekki-ekki-pitang-zoom-boing.”",
  "Hold tight… a man is being chased by a giant animated monster.",
  "Loading… a historian is being abruptly cut off mid-sentence.",
  "Please wait… a man is attempting to count to three but keeps reaching five.",
  "One moment… a knight is bravely approaching a castle full of rude Frenchmen.",
  "Hold on… a man is trying to identify the airspeed of a coconut.",
  "Loading… a group of knights is searching for a holy object they misplaced.",
  "Please wait… a man is being told to “run away!” repeatedly.",
  "One moment… a minstrel is singing an uncomfortably detailed song.",
  "Hold tight… a man is being pestered by a very determined black knight.",
  "Loading… a man is insisting it’s just a flesh wound.",
  "Please wait… a man is attempting to cross a bridge of death.",
  "One moment… a troll is asking very specific questions.",
  "Hold on… a man is arguing about the difference between African and European swallows.",
  "Loading… a knight is being taunted for having a silly name.",
  "Please wait… a man is trying to avoid being turned into a newt.",
  "One moment… a group of villagers is shouting “Burn her!” enthusiastically.",
  "Hold tight… a man is trying to determine if coconuts migrate.",
  "Loading… a man is being followed by a suspiciously large foot.",
  "Please wait… a man is trying to find the holy hand grenade.",
  "One moment… instructions are being read very slowly and incorrectly.",
  "Hold on… a man is counting to three with great difficulty.",
  "Loading… a rabbit is being described as “most foul and cruel.”",
  "Please wait… a man is bravely running away again.",
  "One moment… a knight is shouting “Ni!” at innocent bystanders.",
  "Hold tight… a man is trying to avoid being smacked with a herring.",
  "Loading… a man is being lectured by a very serious narrator.",
  "Please wait… a man is trying to determine if he’s in the wrong sketch.",
  "One moment… a group of knights is demanding yet another shrubbery.",
  "Hold on… a man is insisting this message is still completely different.",
  "Loading… a man is attempting to end the sketch but failing.",
  "Please wait… a giant foot is preparing the finale."
];
        private static readonly IReadOnlyList<Spinner> _spinners = [
    Spinner.Known.Default,
    Spinner.Known.Ascii,
    Spinner.Known.Dots,
    Spinner.Known.Dots2,
    Spinner.Known.Dots3,
    Spinner.Known.Dots4,
    Spinner.Known.Dots5,
    Spinner.Known.Dots6,
    Spinner.Known.Dots7,
    Spinner.Known.Dots8,
    Spinner.Known.Dots9,
    Spinner.Known.Dots10,
    Spinner.Known.Dots11,
    Spinner.Known.Dots12,
    Spinner.Known.Dots13,
    Spinner.Known.Dots14,
    Spinner.Known.Dots8Bit,
    Spinner.Known.DotsCircle,
    Spinner.Known.Sand,
    Spinner.Known.Line,
    Spinner.Known.Line2,
    Spinner.Known.Pipe,
    Spinner.Known.SimpleDots,
    Spinner.Known.SimpleDotsScrolling,
    Spinner.Known.Star,
    Spinner.Known.Star2,
    Spinner.Known.Flip,
    Spinner.Known.Hamburger,
    Spinner.Known.GrowVertical,
    Spinner.Known.GrowHorizontal,
    Spinner.Known.Balloon,
    Spinner.Known.Balloon2,
    Spinner.Known.Noise,
    Spinner.Known.Bounce,
    Spinner.Known.BoxBounce,
    Spinner.Known.BoxBounce2,
    Spinner.Known.Triangle,
    Spinner.Known.Binary,
    Spinner.Known.Arc,
    Spinner.Known.Circle,
    Spinner.Known.SquareCorners,
    Spinner.Known.CircleQuarters,
    Spinner.Known.CircleHalves,
    Spinner.Known.Squish,
    Spinner.Known.Toggle,
    Spinner.Known.Toggle2,
    Spinner.Known.Toggle3,
    Spinner.Known.Toggle4,
    Spinner.Known.Toggle5,
    Spinner.Known.Toggle6,
    Spinner.Known.Toggle7,
    Spinner.Known.Toggle8,
    Spinner.Known.Toggle9,
    Spinner.Known.Toggle10,
    Spinner.Known.Toggle11,
    Spinner.Known.Toggle12,
    Spinner.Known.Toggle13,
    Spinner.Known.Arrow,
    Spinner.Known.Arrow2,
    Spinner.Known.Arrow3,
    Spinner.Known.BouncingBar,
    Spinner.Known.BouncingBall,
    Spinner.Known.Smiley,
    Spinner.Known.Monkey,
    Spinner.Known.Hearts,
    Spinner.Known.Clock,
    Spinner.Known.Earth,
    Spinner.Known.Material,
    Spinner.Known.Moon,
    Spinner.Known.Runner,
    Spinner.Known.Pong,
    Spinner.Known.Shark,
    Spinner.Known.Dqpb,
    Spinner.Known.Weather,
    Spinner.Known.Christmas,
    Spinner.Known.Grenade,
    Spinner.Known.Point,
    Spinner.Known.Layer,
    Spinner.Known.BetaWave,
    Spinner.Known.FingerDance,
    Spinner.Known.FistBump,
    Spinner.Known.SoccerHeader,
    Spinner.Known.Mindblown,
    Spinner.Known.Speaker,
    Spinner.Known.OrangePulse,
    Spinner.Known.BluePulse,
    Spinner.Known.OrangeBluePulse,
    Spinner.Known.TimeTravel,
    Spinner.Known.Aesthetic,
    Spinner.Known.DwarfFortress
];
        private static readonly IReadOnlyList<string> _rainbowColors = [
"#FF0000","#FF0A00","#FF1400","#FF1E00","#FF2800","#FF3200","#FF3C00","#FF4600",
"#FF5000","#FF5A00","#FF6400","#FF6E00","#FF7800","#FF8200","#FF8C00","#FF9600",
"#FFA000","#FFAA00","#FFB400","#FFBE00","#FFC800","#FFD200","#FFDC00","#FFE600",
"#FFF000","#FFFA00","#F7FF00","#EDFF00","#E3FF00","#D9FF00","#CFFF00","#C5FF00",
"#BBFF00","#B1FF00","#A7FF00","#9DFF00","#93FF00","#89FF00","#7FFF00","#75FF00",
"#6BFF00","#61FF00","#57FF00","#4DFF00","#43FF00","#39FF00","#2FFF00","#25FF00",
"#1BFF00","#11FF00","#07FF00","#00FF04","#00FF0E","#00FF18","#00FF22","#00FF2C",
"#00FF36","#00FF40","#00FF4A","#00FF54","#00FF5E","#00FF68","#00FF72","#00FF7C",
"#00FF86","#00FF90","#00FF9A","#00FFA4","#00FFAE","#00FFB8","#00FFC2","#00FFCC",
"#00FFD6","#00FFE0","#00FFEA","#00FFF4","#00FFFE","#00F6FF","#00ECFF","#00E2FF",
"#00D8FF","#00CEFF","#00C4FF","#00BAFF","#00B0FF","#00A6FF","#009CFF","#0092FF",
"#0088FF","#007EFF","#0074FF","#006AFF","#0060FF","#0056FF","#004CFF","#0042FF",
"#0038FF","#002EFF","#0024FF","#001AFF","#0010FF","#0006FF","#0200FF","#0C00FF",
"#1600FF","#2000FF","#2A00FF","#3400FF","#3E00FF","#4800FF","#5200FF","#5C00FF",
"#6600FF","#7000FF","#7A00FF","#8400FF","#8E00FF","#9800FF","#A200FF","#AC00FF",
"#B600FF","#C000FF","#CA00FF","#D400FF","#DE00FF","#E800FF","#F200FF","#FC00FF",
"#FF00F6","#FF00EC","#FF00E2","#FF00D8","#FF00CE","#FF00C4","#FF00BA","#FF00B0",
"#FF00A6","#FF009C","#FF0092","#FF0088","#FF007E","#FF0074","#FF006A","#FF0060",
"#FF0056","#FF004C","#FF0042","#FF0038","#FF002E","#FF0024","#FF001A","#FF0010",
"#FF0006","#FF0000","#FF0A00","#FF1400","#FF1E00","#FF2800","#FF3200","#FF3C00",
"#FF4600","#FF5000","#FF5A00","#FF6400","#FF6E00","#FF7800","#FF8200","#FF8C00",
"#FF9600","#FFA000","#FFAA00","#FFB400","#FFBE00","#FFC800","#FFD200","#FFDC00",
"#FFE600","#FFF000","#FFFA00","#F7FF00","#EDFF00","#E3FF00","#D9FF00","#CFFF00",
"#C5FF00","#BBFF00","#B1FF00","#A7FF00","#9DFF00","#93FF00","#89FF00","#7FFF00",
"#75FF00","#6BFF00","#61FF00","#57FF00","#4DFF00","#43FF00","#39FF00","#2FFF00",
"#25FF00","#1BFF00","#11FF00","#07FF00","#00FF04","#00FF0E","#00FF18","#00FF22",
"#00FF2C","#00FF36","#00FF40","#00FF4A","#00FF54","#00FF5E","#00FF68","#00FF72",
"#00FF7C","#00FF86","#00FF90","#00FF9A","#00FFA4","#00FFAE","#00FFB8","#00FFC2",
"#00FFCC","#00FFD6","#00FFE0","#00FFEA","#00FFF4","#00FFFE","#00F6FF","#00ECFF",
"#00E2FF","#00D8FF","#00CEFF","#00C4FF","#00BAFF","#00B0FF","#00A6FF","#009CFF",
"#0092FF","#0088FF","#007EFF","#0074FF","#006AFF","#0060FF","#0056FF","#004CFF",
"#0042FF","#0038FF","#002EFF","#0024FF","#001AFF","#0010FF","#0006FF","#0200FF",
"#0C00FF","#1600FF","#2000FF","#2A00FF","#3400FF","#3E00FF","#4800FF","#5200FF",
"#5C00FF","#6600FF","#7000FF","#7A00FF","#8400FF","#8E00FF","#9800FF","#A200FF",
"#AC00FF","#B600FF","#C000FF","#CA00FF","#D400FF","#DE00FF","#E800FF","#F200FF",
"#FC00FF","#FF00F6","#FF00EC","#FF00E2","#FF00D8","#FF00CE","#FF00C4","#FF00BA",
"#FF00B0","#FF00A6","#FF009C","#FF0092","#FF0088","#FF007E","#FF0074","#FF006A",
"#FF0060","#FF0056","#FF004C","#FF0042","#FF0038","#FF002E","#FF0024","#FF001A",
"#FF0010","#FF0006"
];
    }
}
