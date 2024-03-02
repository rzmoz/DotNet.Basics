using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace DotNet.Basics.Cli
{
    public class CliHost
    {
        private static readonly string _entryNamespace = Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty;
        private const string _defaultSystemConsoleTemplate = "{Message:lj}{NewLine}{Exception}";

        private readonly string[] _args;
        public bool IsDebug { get; }
        public bool IsHeadless { get; }
        public bool IsADO { get; }

        public CliHost(string[] args)
        {
            _args = args;
            IsDebug = Is("debug");
            IsHeadless = Is("headless");
            IsADO = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("System.TeamProjectId"));

            //init log
            SetGlobalLogger(conf => conf.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: _defaultSystemConsoleTemplate, restrictedToMinimumLevel: IsDebug ? LogEventLevel.Verbose : LogEventLevel.Information)
                .WriteTo.Debug(outputTemplate: _defaultSystemConsoleTemplate, restrictedToMinimumLevel: LogEventLevel.Verbose));
        }

        public CliHost SetGlobalLogger(Func<LoggerConfiguration, LoggerConfiguration> configureGlobal)
        {
            Log.Logger = configureGlobal(new LoggerConfiguration()).CreateLogger();
            return this;
        }
        public void Run(Action run)
        {
            Run(() =>
            {
                run();
                return 0;
            });
        }
        public int Run(Func<int> run)
        {
            try
            {
                LogApplicationEvent("Initializing...");
                PauseForDebugger();
                run();
                return -0;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, ex.Message);
                LogApplicationEvent("Failed and aborting...");
                return -1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        public Task RunAsync(Func<Task> runAsync)
        {
            return RunAsync(async () =>
            {
                await runAsync().ConfigureAwait(false);
                return 0;
            });
        }
        public async Task<int> RunAsync(Func<Task<int>> runAsync)
        {
            try
            {
                LogApplicationEvent("Initializing...");
                if (ShouldPauseForDebugger())
                    PauseForDebugger();
                return await runAsync();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, ex.Message);
                LogApplicationEvent("Failed and aborting...");
                return -1;
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }
        }

        private static void LogApplicationEvent(string @event)
        {
            Log.Information($">>>>>>>>>> {{serviceName}} {@event} <<<<<<<<<<", _entryNamespace);
        }

        internal bool ShouldPauseForDebugger()
        {
            return IsDebug && !IsHeadless;
        }

        private bool Is(string key)
        {
            return _args.Any(a => a.Replace("-", string.Empty).Equals(key, StringComparison.OrdinalIgnoreCase));
        }

        public static void PauseForDebugger()
        {
            Console.WriteLine($"Paused for debug. DisplayName: {Process.GetCurrentProcess().ProcessName} | PID: {Process.GetCurrentProcess().Id}. Press [ENTER] to continue..");
            Console.ReadLine();
        }
    }
}