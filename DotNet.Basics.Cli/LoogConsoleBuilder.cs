using System;
using System.Collections.Generic;
using DotNet.Basics.Serilog;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Cli
{
    public class LoogConsoleBuilder(string[] args)
    {
        private Action<LoogConsoleOptions>? _configureOptions;
        private Action<IServiceCollection>? _configureServices;
        private Func<IServiceCollection>? _createServiceCollection;
        private Func<IReadOnlyList<string>, IReadOnlyDictionary<string, string>>? _argsParser;

        public LoogConsoleHost Build()
        {
            var options = new LoogConsoleOptions(args, _argsParser);
            _configureOptions?.Invoke(options);
            var serviceCollection = _createServiceCollection?.Invoke() ?? new ServiceCollection();
            _configureServices?.Invoke(serviceCollection);
            serviceCollection.AddDiagnosticsWithSerilogDevConsole(verbose: options.Verbose, ado: options.ADO, longRunningOperationsPingInterval: options.LongRunningOperationsPingInterval);
            options.Services = serviceCollection.BuildServiceProvider();
            return new LoogConsoleHost(options);
        }

        public LoogConsoleBuilder Services(Action<IServiceCollection> services, Func<IServiceCollection>? createServiceCollection = null)
        {
            _configureServices = services;
            _createServiceCollection = createServiceCollection;
            return this;
        }
        public LoogConsoleBuilder ArgsParser(Func<IReadOnlyList<string>, IReadOnlyDictionary<string, string>> argsParser)
        {
            _argsParser = argsParser;
            return this;
        }
        public LoogConsoleBuilder Configure(Action<LoogConsoleOptions> configure)
        {
            _configureOptions = configure;
            return this;
        }
    }
}
