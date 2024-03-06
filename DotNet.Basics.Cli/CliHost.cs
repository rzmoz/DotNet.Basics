using System;
using System.Reflection;
using System.Threading.Tasks;
using DotNet.Basics.Serilog;
using Serilog;
using Serilog.Events;

namespace DotNet.Basics.Cli
{
    public class CliHost
    {
        private static readonly string _entryNamespace = Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty;

        public CliHost(string[] args)
        {
            var argsSwitches = new ArgsConsoleSwitches(args);
            if (argsSwitches.ShouldPauseForDebugger())
                argsSwitches.PauseForDebugger();

            //init log
            SetGlobalLogger(conf =>
            {
                conf = conf.WriteTo.DevConsole(argsSwitches.IsDebug, argsSwitches.IsADO);
#if DEBUG
                conf = conf.WriteTo.Debug(restrictedToMinimumLevel: LogEventLevel.Verbose);
#endif
                return conf;
            });

            Log.Verbose($"{nameof(argsSwitches.IsDebug)}: {{IsDebug}}", argsSwitches.IsDebug);
            Log.Verbose($"{nameof(argsSwitches.IsHeadless)}: {{IsHeadless}}", argsSwitches.IsHeadless);
            Log.Verbose($"{nameof(argsSwitches.IsADO)}: {{IsADO}}", argsSwitches.IsADO);
        }

        public CliHost SetGlobalLogger(Func<LoggerConfiguration, LoggerConfiguration> configureGlobal)
        {
            Log.Logger = configureGlobal(new LoggerConfiguration().MinimumLevel.Verbose()).CreateLogger();
            return this;
        }

        public void Run(Action<ILogger> run)
        {
            Run(log =>
            {
                run(log);
                return 0;
            });
        }
        public int Run(Func<ILogger, int> run)
        {
            try
            {
                LogApplicationEvent("Initializing...");
                run(Log.Logger);
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
        public Task RunAsync(Func<ILogger, Task> runAsync)
        {
            return RunAsync(async log =>
            {
                await runAsync(log).ConfigureAwait(false);
                return 0;
            });
        }
        public async Task<int> RunAsync(Func<ILogger, Task<int>> runAsync)
        {
            try
            {
                LogApplicationEvent("Initializing...");
                return await runAsync(Log.Logger);
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
    }
}