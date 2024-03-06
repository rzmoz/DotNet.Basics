using System;
using System.Diagnostics;
using System.Linq;

namespace DotNet.Basics.Cli
{
    public class ArgsConsoleSwitches
    {
        public ArgsConsoleSwitches(string[] args, bool isDebug = false, bool isHeadless = false, bool isAdo = false)
        {
            Args = args;
            IsDebug = Is("debug") || isDebug;
            IsHeadless = Is("headless") || isHeadless;
            IsADO = Is("ado") || isAdo;
        }
        public string[] Args { get; }
        public bool IsDebug { get; }
        public bool IsHeadless { get; }
        public bool IsADO { get; }

        public ArgsConsoleSwitches WithTryWaitForDebugger()
        {
            if (ShouldPauseForDebugger())
                PauseForDebugger();
            return this;
        }

        public bool ShouldPauseForDebugger()
        {
            return !IsHeadless && !IsADO && IsDebug;
        }

        public void PauseForDebugger()
        {
            Console.WriteLine($"Paused for debug. DisplayName: {Process.GetCurrentProcess().ProcessName} | PID: {Process.GetCurrentProcess().Id}. Press [ENTER] to continue..");
            Console.ReadLine();
        }

        private bool Is(string key)
        {
            return Args.Any(a => a.Replace("-", string.Empty).Equals(key, StringComparison.OrdinalIgnoreCase));
        }
    }
}
