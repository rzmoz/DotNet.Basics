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
            if (key.Length < 0)
                throw new ArgumentException($"{nameof(key)} is empty string");

            if (args.IsSet(key, stringComparison))
                return true;
            if (firstCharIsShortKey)
                return args.IsSet(key[0].ToString(), stringComparison);
            return false;
        }

        private static bool IsSet(this string[] args, string key, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (key.Length < 0)
                throw new ArgumentException($"{nameof(key)} is empty string");

            return args.Any(a =>
            {
                foreach (var configurationSwitchFlag in ConfigurationSwitchFlags)
                {
                    if (a.Equals($"{configurationSwitchFlag}{key}", stringComparison))
                        return true;
                }

                return false;
            });
        }

        public static void PauseIfDebug(this string[] args, string debugFlag = "debug")
        {
            if (string.IsNullOrWhiteSpace(debugFlag) || debugFlag.Length < 1)
                return;

            if (args.IsSet(debugFlag, firstCharIsShortKey: true))
            {
                Console.WriteLine($"Paused for debug. Name: {Process.GetCurrentProcess().ProcessName} | PID: {Process.GetCurrentProcess().Id}. Press [ENTER] to continue..");
                Console.ReadLine();
            }
        }
    }
}
