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
    public class CliHostBuilder
    {
        private List<Action<IServiceCollection>> _configureServices = new();
        private Action<Exception?> _defaultExceptionHandling;
        private Action<Exception?> _spectreExceptionHandling;
        private List<Action<CommandApp>> _appActions = new();
        private readonly Func<IServiceCollection>? _serviceCollectionFactory;

        public CliHostBuilder(Func<IServiceCollection>? serviceCollectionFactory = null)
        {
            _defaultExceptionHandling = DefaultExceptionHandling;
            _spectreExceptionHandling = SpectreExceptionHandling;
            _serviceCollectionFactory = serviceCollectionFactory;
        }

        public CliHostBuilder WithDefaultExceptionHandling(Action<Exception?> exceptionHandling)
        {
            _defaultExceptionHandling = exceptionHandling ?? throw new ArgumentNullException(nameof(exceptionHandling));
            return this;
        }
        public CliHostBuilder WithSpectreExceptionHandling(Action<Exception?> exceptionHandling)
        {
            _spectreExceptionHandling = exceptionHandling ?? throw new ArgumentNullException(nameof(exceptionHandling));
            return this;
        }

        public CliHostBuilder WithServices(Action<IServiceCollection> configure)
        {
            _configureServices.Add(configure);
            return this;
        }
        public CliHostBuilder WithCommand<T>(bool isDefault = false, string? name = null, Func<Type, bool>? commandFilter = null, Func<Type, bool>? settingsFilter = null) where T : class, ICommand
        {
            WithServices(services => services.AddCommandsAndSettings<T>(commandFilter, settingsFilter));

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
        public CliHost Build()
        {
            if (_appActions.Count == 0)
                throw new ApplicationException("No commands added. Call WithCommand at least once");

            var appInfo = new CliInfo
            {
                ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty,
                ApplicationVersion = Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString() ?? string.Empty
            };
            var logger = new DevConsole();
            var app = InitApp(logger);
            app.Configure(c =>
            {
                c.SetApplicationName(appInfo.ApplicationName);
                c.SetApplicationVersion(appInfo.ApplicationVersion);
            });
            _appActions.ForEach(a => a(app));
            return new CliHost(app, DefaultGlobalExceptionHandling, appInfo, logger);
        }
        private CommandApp InitApp(DevConsole log)
        {
            var services = _serviceCollectionFactory?.Invoke() ?? new ServiceCollection();
            services.AddSingleton(log);
            services.AddSingleton<ILogger>(s => s.GetService<DevConsole>()!);
            _configureServices.ForEach(s => s.Invoke(services));
            var registrar = new TypeRegistrar(services);
            var app = new CommandApp(registrar);
            app.Configure(c =>
            {
                c.Settings.CaseSensitivity = CaseSensitivity.None;
                c.Settings.StrictParsing = false;
                c.Settings.ValidateExamples = true;
                c.PropagateExceptions();
                c.SetInterceptor(new DevConsoleInterceptor(log));
            });
            return app;
        }

        protected virtual int DefaultGlobalExceptionHandling(Exception? e)
        {
            if (e?.GetType().Namespace?.StartsWith(typeof(IAnsiConsole).Namespace!) ?? false)
                _spectreExceptionHandling(e);
            else
                _defaultExceptionHandling(e);
            if (int.TryParse(e?.GetType().GetProperty("ExitCode")?.GetValue(e)?.ToString(), out int result))
                return result;
            return -1337;
        }

        private void SpectreExceptionHandling(Exception? e)
        {
            if (e == null)
                return;
            AnsiConsole.Write(new Text($"Error in args:", ANSIExtensions.Theme.GetStyle(LogLevel.Critical)));
            AnsiConsole.Write(new Text($" {e.Message}", ANSIExtensions.Theme.GetStyle(LogLevel.Error)));
            AnsiConsole.Write(Text.NewLine);
        }
        private void DefaultExceptionHandling(Exception? e)
        {
            if (e == null)
                return;
            AnsiConsole.WriteException(e, GeneralExceptionSettings);
        }

        protected virtual ExceptionSettings GeneralExceptionSettings { get; } = new ExceptionSettings
        {
            Format = ExceptionFormats.ShortenTypes | ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks,
            Style = new ExceptionStyle
            {
                Exception = new Style(Color.Red),
                Message = new Style(Color.White),
                LineNumber = new Style(Color.Blue),
            }
        };
    }
}