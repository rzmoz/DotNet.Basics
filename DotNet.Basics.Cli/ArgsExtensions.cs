using System;
using System.Diagnostics;
using System.Linq;

namespace DotNet.Basics.Cli
{
    public static class ArgsExtensions
    {
        public static readonly char[] ConfigurationSwitchFlags = { '-', '/' };
        
        public static bool IsSet(this string[] args, string key, bool firstCharIsShortKey = true, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            var strippedKey = key.TrimStart(ConfigurationSwitchFlags);

            //full match
            if (args.Any(a => a.TrimStart(ConfigurationSwitchFlags).Equals(strippedKey, stringComparison)))
                return true;

            if (firstCharIsShortKey == false)
                return false;

            var shortKey = char.ToLowerInvariant(strippedKey[0]);

            return args.Any(a =>
            {
                var loweredArg = char.ToLowerInvariant(a.TrimStart(ConfigurationSwitchFlags)[0]);
                return loweredArg == shortKey;
            });
        }

        public static void PauseIfDebug(this string[] args, string debugFlag = "debug")
        {
            if (args.IsSet(debugFlag))
            {
                Console.WriteLine($"Paused for debug. Name: {Process.GetCurrentProcess().Id} | PID: {Process.GetCurrentProcess().ProcessName}. Press [ENTER] to continue..");
                Console.ReadLine();
            }
        }
    }
}
