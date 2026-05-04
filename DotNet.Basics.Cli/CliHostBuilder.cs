using DotNet.Basics.Cli.Logging;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;

namespace DotNet.Basics.Cli
{
    public class CliHostBuilder(Func<IServiceCollection>? serviceCollectionFactory = null)
    {
        private List<Action<IServiceCollection>> _configureServices = new();
        private Func<Exception, int> _globalExceptionHandling = DefaultGlobalExceptionHandling;
        private static readonly ExceptionSettings _generalExceptionSettings = new ExceptionSettings
        {
            Format = ExceptionFormats.ShortenTypes | ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks,
            Style = new ExceptionStyle
            {
                Exception = new Style(Color.White, Color.DarkRed),
                Message = new Style(Color.Red),
                LineNumber = new Style(Color.Blue),
            }
        };
        private static readonly ExceptionSettings _cliSpectreExceptionSettings = new ExceptionSettings
        {
            Format = ExceptionFormats.ShortenTypes | ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks | ExceptionFormats.NoStackTrace,
            Style = new ExceptionStyle
            {
                Exception = new Style(Color.White, Color.DarkRed),
                Message = new Style(Color.Red),
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
        public CliHost Build<T>(string? name = null, bool isDefault = true) where T : CliCommand<CliCommandSettings>
        {
            return Build<T, CliCommandSettings>(name, isDefault);
        }
        public CliHost Build<T, TK>(string? name = null, bool isDefault = true) where T : CliCommand<TK> where TK : CliCommandSettings
        {
            WithServices(services =>
            {
                services.AddCommandsAndSettings<T>();
                services.AddCommandsAndSettings<TK>();
            });
            var app = InitApp();
            app.Configure(c => c.AddCommand<T>((name ?? typeof(T).Name.RemoveSuffix("Command")).ToLower()));
            if (isDefault)
                app.SetDefaultCommand<T>();
            return new CliHost(app, _globalExceptionHandling);
        }
        public CliHost Build(Action<IConfigurator> configureCommands)
        {
            var app = InitApp();
            app.Configure(c => configureCommands(c));
            return new CliHost(app, _globalExceptionHandling);
        }
        private CommandApp InitApp()
        {
            var services = serviceCollectionFactory?.Invoke() ?? new ServiceCollection();
            var eventLogger = new EventLogger();
            services.AddSingleton<ILogger>(eventLogger);
            _configureServices.ForEach(s => s.Invoke(services));
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
