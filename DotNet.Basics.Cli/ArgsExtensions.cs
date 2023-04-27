using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Cli
{
    public static class ArgsExtensions
    {
        public static readonly char[] ConfigurationSwitchFlags = { DefaultArgsSwitch, '/' };
        public const string IsSetValue = "___IsSet___";

        public const char DefaultArgsSwitch = '-';
        public const string MicrosoftExtensionsArgsSwitch = "--";
        public static readonly string EnvironmentsKey = "environments";

        private static readonly string[] _environmentAliases = {
            "env",
            "envs",
            "environment",
            EnvironmentsKey
        };

        public static bool IsArgSwitch(this string arg)
        {
            return !string.IsNullOrEmpty(arg) && ConfigurationSwitchFlags.Contains(arg[0]);
        }

        public static IEnumerable<string> EnsureEnvironmentsAreDistinct(this IEnumerable<string> args)
        {
            if (args == null)
                yield break;
            var previousWasEnvironmentsKey = false;
            foreach (var arg in args)
            {
                if (previousWasEnvironmentsKey)
                    yield return arg.Split('|').Select(env =>
                    {
                        env = env.Trim(' ').ToLowerInvariant();
                        var envBuilder = new StringBuilder(env.Length);
                        envBuilder.Append(char.ToUpper(env[0]));
                        if (env.Length > 1)
                            envBuilder.Append(env.Substring(1));

                        return envBuilder.ToString();
                    }).Distinct(StringComparer.InvariantCultureIgnoreCase).JoinString(); //return distinct environments
                else
                    yield return arg;

                previousWasEnvironmentsKey = _environmentAliases.Contains(arg.TrimStart(DefaultArgsSwitch), StringComparer.InvariantCultureIgnoreCase);
            }
        }

        public static IEnumerable<string> CleanArgsForCli(this IEnumerable<string> args)
        {
            //ensure switches are consistent and adhere to Microsoft notation
            return args
                    .Select(arg =>//normalize switches
                    {
                        if (arg.IsArgSwitch())
                            arg = arg.TrimStart(DefaultArgsSwitch).EnsurePrefix(MicrosoftExtensionsArgsSwitch);
                        return arg;
                    });
        }

        public static IEnumerable<string> EnsureFlagsHaveValue(this IEnumerable<string> args)
        {
            if (args == null)
                yield break;

            //ensure flags (switches without values) have a value in order to use it in IConfigRoot
            var expectSwitch = true;
            var hasArgs = false;
            foreach (var arg in args)
            {
                hasArgs = true;
                if (string.IsNullOrEmpty(arg))
                    continue; //empty entries are ignored

                if (arg.IsArgSwitch()) //is switch
                {
                    if (expectSwitch)
                    {
                        yield return arg;
                    }
                    else
                    {
                        yield return IsSetValue;//add value for flag since it's not present
                        yield return arg;
                    }

                    expectSwitch = false;
                }
                else //were in values
                {
                    yield return arg;
                    expectSwitch = true;
                }
            }

            if (hasArgs && expectSwitch == false)//return flag value since previous item was switch thus flag
                yield return IsSetValue;
        }

        public static bool IsSet(this IReadOnlyList<string> args, string key, bool firstCharIsShortKey = false, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
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

        private static bool IsSet(this IReadOnlyList<string> args, string key, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
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
