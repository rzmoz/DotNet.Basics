using System;
using System.Linq;
using DotNet.Basics.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Cli
{
    public class CliHostBuilder(string[] args, IArgsParser argsParser, Func<IServiceCollection>? serviceCollectionFactory = null)
    {
        private Action<CliHostBuilderOptions, IServiceCollection>? _configureServices = (_, _) => { };
        private Action<CliHostBuilderOptions>? _configureOptions = _ => { };

        public CliHostBuilder(string[] args, bool firstEntryIsVerb, Func<IServiceCollection>? serviceCollectionFactory = null)
        : this(args, new ArgsDefaultParser(firstEntryIsVerb), serviceCollectionFactory)
        { }
        public CliHost Build()
        {
            if (args.Any(a => a.Contains("-debug", StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine($"Pausing to attach debugger [{Environment.ProcessId}]. Press enter to continue");
                Console.ReadLine();
            }

            var options = new CliHostBuilderOptions(argsParser.Parse(args));
            _configureOptions?.Invoke(options);

            var services = serviceCollectionFactory?.Invoke() ?? new ServiceCollection();

            if (options.WithDevColoredConsole)
                services.AddDevConsole(o => o.IsAdo = options.Args.ADO);

            services.AddSingleton<LongRunningOperations>();
            _configureServices?.Invoke(options, services);

            var hostOptions = new CliHostOptions(options, services.BuildServiceProvider());
            return new CliHost(hostOptions);
        }

        public CliHostBuilder WithServices(Action<IServiceCollection>? configure)
        {
            _configureServices = (_, s) => configure?.Invoke(s);
            return this;
        }
        public CliHostBuilder WithServices(Action<CliHostBuilderOptions, IServiceCollection>? configure)
        {
            _configureServices = configure;
            return this;
        }

        public CliHostBuilder WithOptions(Action<CliHostBuilderOptions>? configureOptions = null)
        {
            _configureOptions = configureOptions;
            return this;
        }
    }
}
