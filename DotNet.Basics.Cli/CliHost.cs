using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;
using Microsoft.Extensions.Configuration;

namespace DotNet.Basics.Cli
{
    public class CliHost : ICliConfiguration
    {
        public CliHost(string[] args, IConfigurationRoot config, ILogDispatcher log)
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
        public ILogDispatcher Log { get; }
        public IReadOnlyCollection<string> Environments => Config.Environments();

        public bool IsSet(string key) => Config.IsSet(key) || Args.IsSet(key);
        public bool HasValue(string key) => Config.HasValue(key);

        public virtual Task<int> RunAsync(string name, Func<ICliConfiguration, ILogDispatcher, Task<int>> asyncAction)
        {
            return RunAsync(name, asyncAction, 1.Minutes());
        }
        public virtual async Task<int> RunAsync(string name, Func<ICliConfiguration, ILogDispatcher, Task<int>> asyncAction, TimeSpan longRunningOperationsPingInterval)
        {
            if (asyncAction == null) throw new ArgumentNullException(nameof(asyncAction));

            try
            {
                LongRunningOperations.Init(Log, longRunningOperationsPingInterval);

                return await LongRunningOperations.StartAsync(name, async () =>
                    await asyncAction.Invoke(this, Log).ConfigureAwait(false))
                    .ConfigureAwait(false);
            }
            catch (CliException e)
            {
                Log.Error(e.Message.Highlight(), e.LogOptions == LogOptions.IncludeStackTrace ? e : null);
                return e.ExitCode;
            }
            catch (Exception e)
            {
                Log.Error(e.Message.Highlight(), e);
                return 500;
            }
        }
    }
}