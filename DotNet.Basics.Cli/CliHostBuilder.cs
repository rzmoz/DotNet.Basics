using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DotNet.Basics.Cli.ConsoleOutput;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Cli
{
    public class CliHostBuilder
    {
        private readonly string[] _args;
        public const char DefaultArgsSwitch = '-';
        public const string MicrosoftExtensionsArgsSwitch = "--";

        private readonly IConfigurationBuilder _configurationBuilder;
        private readonly ILogDispatcher _log;
        private readonly AppInfo _appInfo;

        public CliHostBuilder(string[] args, Func<ArgsSwitchMappings> switchMappings = null, Type classThatContainStaticVoidMain = null)
        {
            _args = args;
            _configurationBuilder = InitConfigurationBuilder(switchMappings);
            _log = new LogDispatcher();
            _appInfo = new AppInfo(classThatContainStaticVoidMain);
            WithConsole();
        }
        public CliHostBuilder WithConfiguration(Action<IConfigurationBuilder> add)
        {
            _log.Debug($"Entering custom configuration setup");
            add?.Invoke(_configurationBuilder);
            return this;
        }

        public CliHostBuilder WithDiagnosticsTarget(Action<LogLevel, string, Exception> addLogTarget, Action<string, double> addMetricTarget)
        {
            if (addLogTarget != null)
                _log.MessageLogged += addLogTarget.Invoke;

            if (addMetricTarget != null)
                _log.MetricLogged += addMetricTarget.Invoke;

            return this;
        }

        public CliHost Build()
        {
            return new CliHost(_configurationBuilder.Build(), _log);
        }

        private CliHostBuilder WithConsole(ConsoleTheme consoleTheme = null)
        {
            IConsoleWriter console = new ColoredConsoleWriter(consoleTheme);
            if (((ColoredConsoleWriter)console).ConsoleModeProperlySet == false)
                console = new SystemConsoleWriter();

            WithDiagnosticsTarget(console.Write, (name, value) => console.Write(LogLevel.Information, $"[{name} : {value.ToString(CultureInfo.InvariantCulture).Highlight()}]".WithIndent(10)));
            _log.Information($@"Initializing {_appInfo.ToString().Highlight()}");
            _log.Debug($"{console.GetType().Name} logger added as logging target");
            return this;
        }

        private IConfigurationBuilder InitConfigurationBuilder(Func<ArgsSwitchMappings> addSwitchMappings)
        {
            var configArgs = _args.Select(a =>
            {
                if (a.StartsWith(DefaultArgsSwitch.ToString()))
                    a = a.TrimStart(DefaultArgsSwitch).EnsurePrefix(MicrosoftExtensionsArgsSwitch);
                return a;
            }).ToArray();
            
            var switchMappings = new ArgsSwitchMappings();
            switchMappings.AddRange(addSwitchMappings?.Invoke());

            var envConfigRoot = new ConfigurationBuilder().AddCommandLine(configArgs, switchMappings.ToDictionary()).Build();
            var environments = (envConfigRoot["env"] ?? envConfigRoot["envs"] ?? envConfigRoot["environment"] ?? envConfigRoot["environments"])?.Split('|').Select(env => env.Trim()) ?? Enumerable.Empty<string>();

            //configure configuration sources
            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", true, false);
            foreach (var env in environments)
                configBuilder.AddJsonFile($"appSettings.{env}.json", true, false);
            configBuilder.AddCommandLine(configArgs, switchMappings.ToDictionary());

            return configBuilder;
        }
    }
}
