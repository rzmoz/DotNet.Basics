using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace DotNet.Basics.Cli
{
    public class CliArgs
    {
        public CliArgs(string[] args, IConfigurationRoot config)
        {
            Args = args ?? throw new ArgumentNullException(nameof(args));
            Config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public string this[string key] => Config[key];
        public string this[int index] => index < Args.Count ? Args[index] : null;

        public IReadOnlyList<string> Args { get; }
        public IConfigurationRoot Config { get; }
    }
}
