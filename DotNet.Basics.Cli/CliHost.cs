using System;
using System.Collections.Generic;
using DotNet.Basics.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace DotNet.Basics.Cli
{
    public class CliHost : IDisposable
    {
        public CliHost(string[] args, IConfigurationRoot config, LogDispatcher log)
        {
            Args = args ?? throw new ArgumentNullException(nameof(args));
            Config = config ?? throw new ArgumentNullException(nameof(config));
            Log = log;
        }

        public string this[string key, int index] => this[key] ?? this[index];
        public string this[string key] => Config[key];
        public string this[int index] => index < Args.Count ? Args[index] : null;

        public IReadOnlyList<string> Args { get; }
        public IConfigurationRoot Config { get; }
        public LogDispatcher Log { get; }
    

        public void Dispose()
        {
            Log.CloseAndFlush();
        }
    }
}