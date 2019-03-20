using System;
using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Sys;
using Microsoft.Extensions.Configuration;

namespace DotNet.Basics.Cli
{
    public class CliArgs
    {
        private readonly string _argsSwitch;

        public CliArgs(string[] args, IConfigurationRoot config, string argsSwitch)
        {
            _argsSwitch = argsSwitch;
            Args = args ?? throw new ArgumentNullException(nameof(args));
            Config = config ?? throw new ArgumentNullException(nameof(config));
            IsDebug = IsSet("debug");
        }

        public string this[string key] => Config[key];

        public bool IsSet(string key, bool firstCharIsShortKey = true, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            //full name match
            var match = Args.Any(a => a.Equals(key.EnsurePrefix(_argsSwitch), stringComparison));
            if (firstCharIsShortKey)
                match = match || Args.Any(a => a.Equals(key.RemovePrefix(_argsSwitch)[0].ToString().EnsurePrefix(_argsSwitch), stringComparison));
            return match;
        }


        public bool IsDebug { get; }
        public IReadOnlyList<string> Args { get; }
        public IConfigurationRoot Config { get; }
    }
}
