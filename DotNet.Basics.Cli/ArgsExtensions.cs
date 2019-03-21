using System;
using System.Diagnostics;
using Serilog;

namespace DotNet.Basics.Cli
{
    public static class ArgsExtensions
    {
        public static bool IsDebug(this string[] args, string debugFlag = "debug")
        {
            foreach (var rawArg in args)
            {
                if (string.IsNullOrWhiteSpace(rawArg))
                    continue;

                var arg = rawArg.TrimStart(CliArgs.ConfigurationSwitchFlags);
                if (arg.Equals(debugFlag, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (arg.ToLowerInvariant() == char.ToLowerInvariant(debugFlag[0]).ToString())
                    return true;
            }
            return false;
        }

        public static void PauseIfDebug(this string[] args, string debugFlag = "debug")
        {
            if (args.IsDebug())
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.ColoredConsole()
                    .CreateLogger();

                Log.Warning("Paused for debug. PID: {ProcessId} | Name: {ProcessName}. Press {ENTER} to continue..", Process.GetCurrentProcess().Id, Process.GetCurrentProcess().ProcessName, "[ENTER]");
                Console.ReadLine();

                Log.Logger = null;
            }
        }
    }
}
