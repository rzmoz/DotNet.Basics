using System;
using System.Linq;
using DotNet.Basics.Serilog;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;
using Serilog;

namespace DotNet.Basics.Cli
{
    public class CliHostBuilder(string[] args, Func<IServiceCollection>? serviceCollectionFactory = null, IArgsParser? argsParser = null)
    {
        private Action<CliHostBuilderOptions, IServiceCollection>? _configureServices = (_, _) => { };
        private Action<CliHostBuilderOptions>? _configureOptions = _ => { };
        private Action<CliHostBuilderOptions, LoggerConfiguration>? _configureSerilog = (_, _) => { };
        public CliHost Build()
        {
            if (args.Any(a => a.Contains("-debug", StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine($"Pausing to attach debugger [{Environment.ProcessId}]. Press enter to continue");
                Console.ReadLine();
            }

            var options = new CliHostBuilderOptions(args, argsParser ?? new ArgsMsBuildStyleParser());
            _configureOptions?.Invoke(options);

            if (options.WithSerilogDevConsole)
                InitSerilog(options);

            var serviceCollection = serviceCollectionFactory?.Invoke() ?? new ServiceCollection();
            serviceCollection.AddLoogDiagnostics(verbose: options.Args.Verbose, ado: options.Args.ADO);
            _configureServices?.Invoke(options, serviceCollection);
            var services = serviceCollection.BuildServiceProvider();

            var hostOptions = new CliHostOptions(options, services);
            return new CliHost(hostOptions);
        }

        private void InitSerilog(CliHostBuilderOptions o)
        {
            var config = new LoggerConfiguration();
            config.MinimumLevel.Is(o.Args.Verbose ? LogEventLevel.Verbose : LogEventLevel.Information);
            config.WriteTo.DevConsole(verbose: o.Args.Verbose, ado: o.Args.ADO);
            _configureSerilog?.Invoke(o, config);
            Log.Logger = config.CreateLogger();
        }

        public CliHostBuilder WithSerilog(Action<LoggerConfiguration>? configure = null)
        {
            _configureSerilog = (_, s) => configure?.Invoke(s);
            return this;
        }
        public CliHostBuilder WithSerilog(Action<CliHostBuilderOptions, LoggerConfiguration>? configure = null)
        {
            _configureSerilog = configure;
            return this;
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
