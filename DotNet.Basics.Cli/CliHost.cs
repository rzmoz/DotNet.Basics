using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace DotNet.Basics.Cli
{
    public class CliHost
    {
        public CliHost(IConfigurationRoot config, ILogDispatcher log)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            Log = log;
        }

        public string this[string key, int index] => this[key] ?? this[index];
        public string this[string key] => Config[key];
        public string this[int index] => index < Args.Count ? Args[index] : null;

        public IReadOnlyList<string> Args { get; }
        public IConfigurationRoot Config { get; }
        public ILogDispatcher Log { get; }

        public async Task<int> RunAsync(string name, Func<IConfigurationRoot, ILogDispatcher, Task> asyncAction)
        {
            if (asyncAction == null) throw new ArgumentNullException(nameof(asyncAction));
            try
            {
                Log.Debug($"Invoking RunAsync");
                await asyncAction.Invoke(Config, Log).ConfigureAwait(false);
                return 0;
            }
            catch (CliException e)
            {
                Log.Error(e.Message, e.IgnoreStackTraceInLogOutput ? null : e);
                return (int)HttpStatusCode.InternalServerError;
            }
            catch (Exception e)
            {
                Log.Critical(e.Message, e);
                return 0;
            }
        }
    }
}