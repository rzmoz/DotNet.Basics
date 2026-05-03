using DotNet.Basics.Cli.Logging;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;
using System;

namespace DotNet.Basics.Cli
{
    public class CliHostBuilder(Func<IServiceCollection>? serviceCollectionFactory = null)
    {
        private Action<IServiceCollection>? _configureServices = (_) => { };

        public CliHostBuilder WithServices(Action<IServiceCollection> configure)
        {
            _configureServices = configure;
            return this;
        }
        public CliHost Build<T>(string? name = null, bool isDefault = true) where T : CliCommand<CliCommandSettings>
        {
            return Build<T, CliCommandSettings>((name ?? typeof(T).Name.RemoveSuffix("Command")).ToLower(), isDefault);
        }
        public CliHost Build<T, TK>(string name, bool isDefault = true) where T : CliCommand<TK> where TK : CliCommandSettings
        {
            WithServices(services =>
            {
                services.AddSingleton<TK>();
                services.AddTransient<T>();
            });
            var app = InitApp();
            app.Configure(c => c.AddCommand<T>(name));
            if (isDefault)
                app.SetDefaultCommand<T>();
            return new CliHost(app);

        }

        public CliHost Build(Action<IConfigurator> configureCommands)
        {
            var app = InitApp();
            app.Configure(c => configureCommands(c));
            return new CliHost(app);
        }
        private CommandApp InitApp()
        {
            var services = serviceCollectionFactory?.Invoke() ?? new ServiceCollection();
            var eventLogger = new EventLogger();
            services.AddSingleton<ILogger>(eventLogger);

            _configureServices?.Invoke(services);
            var registrar = new TypeRegistrar(services);
            var app = new CommandApp(registrar);
            app.Configure(c =>
            {
                c.Settings.CaseSensitivity = CaseSensitivity.None;
                c.Settings.StrictParsing = false;
                c.Settings.ValidateExamples = true;
                c.PropagateExceptions();
                c.SetInterceptor(new DevConsoleInterceptor(eventLogger));
            });
            return app;
        }
    }
}
