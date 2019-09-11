using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
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
            Verbose = config[nameof(Verbose)] != null;
        }

        public string this[string key, int index] => this[key] ?? this[index];
        public string this[string key] => Config[key];
        public string this[int index] => index < Args.Count ? Args[index] : null;

        public IReadOnlyList<string> Args { get; }
        public IConfigurationRoot Config { get; }
        public ILogDispatcher Log { get; }
        public IReadOnlyCollection<string> Environments => Config.Environments();
        public bool Verbose { get; }

        public virtual async Task<int> RunAsync(string name, Func<IConfigurationRoot, ILogDispatcher, Task<int>> asyncAction, CliHostOptions options = null)
        {
            if (asyncAction == null) throw new ArgumentNullException(nameof(asyncAction));
            if (options == null)
                options = new CliHostOptions();
            try
            {
                LongRunningOperations.Init(Log, options.LongRunningOperationsPingInterval);
                var exitCode = int.MinValue;
                await LongRunningOperations.StartAsync(name, async () =>
                {
                    exitCode = await asyncAction.Invoke(Config, Log).ConfigureAwait(false);
                }).ConfigureAwait(false);

                return exitCode;
            }
            catch (CliException e)
            {
                options.LogOnCliException?.Invoke(e, Log);
                return options.ReturnCodeOnError;
            }
            catch (Exception e)
            {
                options.LogOnException?.Invoke(e, Log);
                return options.ReturnCodeOnError;
            }
        }
    }
}