using System;
using System.Collections.Generic;
using System.Linq;
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
        
        public IEnumerable<string> GetRange(string key)
        {
            var keyFound = false;

            foreach (var arg in Args)
            {
                if (string.IsNullOrWhiteSpace(arg))
                    continue;

                //match key
                if (arg.ToLowerInvariant().TrimStart('-').TrimStart('/').Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    keyFound = true;
                    continue;
                }

                if (keyFound && (arg.First() == '-' || arg.First() == '/'))
                    break;

                yield return arg;
            }
        }

        public string Get(string key)
        {
            return Config[key];
        }
        public string Get(string key, int index)
        {
            return Config[key] ?? Get(index);
        }

        public string Get(int index)
        {
            return index < Args.Count ? Args[index] : null;
        }

        public IReadOnlyList<string> Args { get; }
        public IConfigurationRoot Config { get; }
    }
}