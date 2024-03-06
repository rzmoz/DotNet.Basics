using System;
using System.Diagnostics;
using System.Linq;

namespace DotNet.Basics.Cli
{
    public class ArgsConsoleSwitches
    {
        public ArgsConsoleSwitches(string[] args)
        {
            Args = args;
            IsDebug = Is("debug");
            IsHeadless = Is("headless");
            IsADO = Is("ado") || EnvironmentVariableExists("System.TeamProjectId") && EnvironmentVariableExists("Build.ArtifactStagingDirectory");
        }
        public string[] Args { get; }
        public bool IsDebug { get; }
        public bool IsHeadless { get; }
        public bool IsADO { get; }

        public bool ShouldPauseForDebugger()
        {
            if (IsHeadless)
                return false;
            if (IsADO)
                return false;
            if (!IsDebug)
                return false;

            return true;
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
        private static bool EnvironmentVariableExists(string name)
        {
            return !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(name));
        }
    }
}
