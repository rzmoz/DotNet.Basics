using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace DotNet.Basics.Cli
{
    public class CliHost
    {
        public CliHost(IReadOnlyList<string> args, IConfigurationRoot config, ILogDispatcher log)
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

        public virtual async Task<int> RunAsync(string name, Func<IConfigurationRoot, ILogDispatcher, Task> asyncAction, CliHostOptions options = null)
        {
            if (asyncAction == null) throw new ArgumentNullException(nameof(asyncAction));
            if (options == null)
                options = new CliHostOptions();
            try
            {
                LongRunningOperations.Init(Log, options.LongRunningOperationsPingInterval);

                await LongRunningOperations.StartAsync(name, async () =>
                {
                    await asyncAction.Invoke(Config, Log).ConfigureAwait(false);
                }).ConfigureAwait(false);

                return 0;
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