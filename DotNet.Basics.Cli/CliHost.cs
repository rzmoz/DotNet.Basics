using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;
using Microsoft.Extensions.Configuration;

namespace DotNet.Basics.Cli
{
    public class CliHost<T> : ICliHost<T>
    {
        public CliHost(T args, string[] rawArgs, IConfigurationRoot config, ILogDispatcher log)
        {
            Args = args;
            RawArgs = rawArgs ?? throw new ArgumentNullException(nameof(rawArgs));
            Config = config ?? throw new ArgumentNullException(nameof(config));
            Log = log ?? LogDispatcher.NullLogger;
        }

        public string this[string key, int index] => this[key] ?? this[index];
        public string this[string key] => Config[key];
        public string this[int index] => index < RawArgs.Count ? RawArgs[index] : null;

        public T Args { get; }
        public IReadOnlyList<string> RawArgs { get; }
        public IConfigurationRoot Config { get; }
        public ILogDispatcher Log { get; }
        public IReadOnlyCollection<string> Environments => Config.Environments();

        public bool IsSet(string key) => Config.IsSet(key) || RawArgs.IsSet(key);
        public bool HasValue(string key) => Config.HasValue(key);

        public virtual Task<int> RunAsync(string name, Func<T, ICliConfiguration, ILogDispatcher, Task<int>> asyncAction)
        {
            return RunAsync(name, asyncAction, 1.Minutes());
        }
        public virtual async Task<int> RunAsync(string name, Func<T, ICliConfiguration, ILogDispatcher, Task<int>> asyncAction, TimeSpan longRunningOperationsPingInterval)
        {
            if (asyncAction == null) throw new ArgumentNullException(nameof(asyncAction));

            try
            {
                LongRunningOperations.Init(Log, longRunningOperationsPingInterval);

                return await LongRunningOperations.StartAsync(name, async () =>
                        await asyncAction.Invoke(Args, this, Log).ConfigureAwait(false))
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
    public class CliHost : CliHost<string[]>
    {
        public CliHost(string[] rawArgs, IConfigurationRoot config, ILogDispatcher log) : base(rawArgs, rawArgs, config, log)
        { }
    }
}