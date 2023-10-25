using System;
using System.Diagnostics;
using System.Linq;

namespace DotNet.Basics.Cli
{
    public static class ArgsExtensions
    {
        public static void PauseIfDebug(this string[] args)
        {
            if (args.Any(a => a.ToLowerInvariant().TrimStart('-').Equals("debug")))
            {
                Console.WriteLine($"Paused for debug. Name: {Process.GetCurrentProcess().ProcessName} | PID: {Process.GetCurrentProcess().Id}. Press [ENTER] to continue..");
                Console.ReadLine();
            }
        }
    }
}
