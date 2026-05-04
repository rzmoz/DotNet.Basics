using DotNet.Basics.Cli.Logging;
using DotNet.Basics.Sys;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DotNet.Basics.Cli
{
    public class CliHostBuilder(Func<IServiceCollection>? serviceCollectionFactory = null)
    {
        private List<Action<IServiceCollection>> _configureServices = new();
        private Func<Exception, int> _globalExceptionHandling = DefaultGlobalExceptionHandling;
        private List<Action<CommandApp>> _appActions = new();

        private static readonly ExceptionSettings _generalExceptionSettings = new ExceptionSettings
        {
            Format = ExceptionFormats.ShortenTypes | ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks,
            Style = new ExceptionStyle
            {
                Exception = new Style(Color.Red),
                Message = new Style(Color.White),
                LineNumber = new Style(Color.Blue),
            }
        };
        private static readonly ExceptionSettings _cliSpectreExceptionSettings = new ExceptionSettings
        {
            Format = ExceptionFormats.ShortenTypes | ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks | ExceptionFormats.NoStackTrace,
            Style = new ExceptionStyle
            {
                Exception = new Style(Color.Red),
                Message = new Style(Color.White),
                LineNumber = new Style(Color.Blue),
            }
        };

        private static int DefaultGlobalExceptionHandling(Exception e)
        {
            var isSpectraException = e.GetType().Namespace?.StartsWith(typeof(IAnsiConsole).Namespace!) ?? false;
            AnsiConsole.WriteException(e, isSpectraException ? _cliSpectreExceptionSettings : _generalExceptionSettings);
            if (int.TryParse(e.GetType().GetProperty("ExitCode")?.GetValue(e)?.ToString(), out int result))
                return result;
            return int.MinValue;
        }

        public CliHostBuilder WithGlobalExceptionHandling(Func<Exception, int> globalExceptionHandling)
        {
            _globalExceptionHandling = globalExceptionHandling ?? throw new ArgumentNullException(nameof(globalExceptionHandling));
            return this;
        }

        public CliHostBuilder WithServices(Action<IServiceCollection> configure)
        {
            _configureServices.Add(configure);
            return this;
        }
        public CliHostBuilder WithCommand<T>(bool isDefault = true, string? name = null) where T : class, ICommand
        {
            name = (name ?? typeof(T).Name).RemoveSuffix("Command").ToLower();
            return WithAppAction(app =>
            {
                app.Configure(ctx => ctx.AddCommand<T>(name));
                if (isDefault)
                    app.SetDefaultCommand<T>();
            });
        }
        public CliHostBuilder WithAppAction(Action<CommandApp> appAction)
        {
            _appActions.Add(appAction);
            return this;
        }
        public CliHost Build(Func<Type, bool>? commandFilter = null, Func<Type, bool>? settingsFilter = null, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                assemblies = [Assembly.GetEntryAssembly()];
            if (_appActions.Count == 0)
                throw new ApplicationException("No commands added. Call WithCommand at least once");

            WithServices(services =>
            {
                services.AddCommandsAndSettings(assemblies, commandFilter, settingsFilter);
            });
            var app = InitApp();
            _appActions.ForEach(a => a(app.app));
            return new CliHost(app.app, app.console, _globalExceptionHandling);
        }
        public CliHost Build(Action<IConfigurator> configureCommands)
        {
            var app = InitApp();
            app.app.Configure(c => configureCommands(c));
            return new CliHost(app.app, app.console, _globalExceptionHandling);
        }
        private (CommandApp app, DevConsoleLogger console) InitApp()
        {
            var services = serviceCollectionFactory?.Invoke() ?? new ServiceCollection();
            var console = new DevConsoleLogger();
            services.AddSingleton(console);
            services.AddSingleton<ILogger>(s => s.GetService<DevConsoleLogger>()!);
            _configureServices.ForEach(s => s.Invoke(services));
            var registrar = new TypeRegistrar(services);
            var app = new CommandApp(registrar);
            app.Configure(c =>
            {
                c.Settings.CaseSensitivity = CaseSensitivity.None;
                c.Settings.StrictParsing = false;
                c.Settings.ValidateExamples = true;
                c.PropagateExceptions();
                c.SetInterceptor(new DevConsoleInterceptor(console));
            });
            return (app, console);
        }
    }
}
