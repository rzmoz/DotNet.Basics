﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using static System.Char;

namespace DotNet.Basics.Cli
{
    public class CliArgs
    {
        public static readonly char[] ConfigurationSwitchFlags = { '-', '/' };

        public CliArgs(string[] args, IConfigurationRoot config)
        {
            Args = args ?? throw new ArgumentNullException(nameof(args));
            Config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public string this[string key] => Config[key];
        public string this[int index] => index < Args.Count ? Args[index] : null;

        public bool IsSet(string key, bool firstCharIsShortKey = true, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            var strippedKey = key.TrimStart(ConfigurationSwitchFlags);

            //full match
            if (Args.Any(a => a.TrimStart(ConfigurationSwitchFlags).Equals(strippedKey, stringComparison)))
                return true;

            if (firstCharIsShortKey == false)
                return false;

            var shortKey = ToLowerInvariant(strippedKey[0]);

            return Args.Any(a =>
            {
                var loweredArg = ToLowerInvariant(a.TrimStart(ConfigurationSwitchFlags)[0]);
                return loweredArg == shortKey;
            });
        }

        public IReadOnlyList<string> Args { get; }
        public IConfigurationRoot Config { get; }
    }
}
