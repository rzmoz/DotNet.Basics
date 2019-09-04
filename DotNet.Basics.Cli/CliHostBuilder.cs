using System;
using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Cli.ConsoleOutput;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;
using Microsoft.Extensions.Configuration;

namespace DotNet.Basics.Cli
{
    public class CliHostBuilder
    {
        private readonly string[] _args;
        private readonly Action<ArgsSwitchMappings> _customSwitchMappings;
        private readonly IList<Action<IConfigurationBuilder>> _configurationActions = new List<Action<IConfigurationBuilder>>();
        public const char DefaultArgsSwitch = '-';
        public const string MicrosoftExtensionsArgsSwitch = "--";

        private readonly ILogDispatcher _log;
        private readonly AppInfo _appInfo;

        public CliHostBuilder(string[] args, Action<ArgsSwitchMappings> customSwitchMappings = null, Type classThatContainStaticVoidMain = null)
        {
            _args = args;
            _customSwitchMappings = customSwitchMappings;

            _log = new LogDispatcher();
            _appInfo = new AppInfo(classThatContainStaticVoidMain);

            WithConsole();
            _log.Information($@"Initializing {_appInfo.ToString().Highlight()} with args {_args.JoinString(" ").Highlight()}");
        }
        public CliHostBuilder WithConfiguration(Action<IConfigurationBuilder> add)
        {
            _configurationActions.Add(add);
            return this;
        }

        public CliHostBuilder WithDiagnosticsTarget(IDiagnosticsTarget diagnosticsTarget)
        {
            if (diagnosticsTarget == null) throw new ArgumentNullException(nameof(diagnosticsTarget));
            if (diagnosticsTarget.LogTarget != null)
                _log.MessageLogged += diagnosticsTarget.LogTarget.Invoke;

            if (diagnosticsTarget.TimingTarget != null)
                _log.TimingLogged += diagnosticsTarget.TimingTarget.Invoke;

            return this;
        }

        public CliHost Build()
        {
            return new CliHost(_args, InitConfigurationRoot(), _log);
        }

        public T BuildCustomHost<T>(Func<string[], IConfigurationRoot, ILogDispatcher, T> build) where T : CliHost
        {
            if (build == null) throw new ArgumentNullException(nameof(build));
            return build(_args, InitConfigurationRoot(), _log);
        }

        private CliHostBuilder WithConsole(ConsoleTheme consoleTheme = null)
        {
            ConsoleWriter console = new ColoredConsoleWriter(consoleTheme);
            if (((ColoredConsoleWriter)console).ConsoleModeProperlySet == false)
                console = new SystemConsoleWriter();

            WithDiagnosticsTarget(console);
            return this;
        }

        protected virtual IConfigurationRoot InitConfigurationRoot()
        {
            var configArgs = _args.Select(a =>
            {
                if (a.StartsWith(DefaultArgsSwitch.ToString()))
                    a = a.TrimStart(DefaultArgsSwitch).EnsurePrefix(MicrosoftExtensionsArgsSwitch);
                return a;
            }).ToArray();

            var switchMappings = new ArgsSwitchMappings(_customSwitchMappings);

            if (switchMappings.Any())
            {
                _log.Verbose($"Adding switch mappings to configuration:");
                foreach (var entry in switchMappings)
                    _log.Verbose($"{entry.Key} => {entry.Value}");
            }

            var envConfigRoot = new ConfigurationBuilder().AddCommandLine(configArgs, switchMappings.ToDictionary()).Build();
            var environments = envConfigRoot.Environments();

            _log.Debug($"Environments: {environments.JoinString()}");

            //configure configuration sources
            _log.Verbose($"Reading config from appSettings.json");
            var configBuilder = new ConfigurationBuilder().AddJsonFile("appSettings.json", true, false);
            foreach (var env in environments)
            {
                _log.Verbose($"Reading config from appSettings.{env}.json");
                configBuilder.AddJsonFile($"appSettings.{env}.json", true, false);
            }

            if (_configurationActions.Any())
                _log.Verbose($"Reading config from custom configuration in host");
            foreach (var configurationAction in _configurationActions)
                configurationAction?.Invoke(configBuilder);

            if (_args.Any())
                _log.Verbose($"Reading config from args");
            configBuilder.AddCommandLine(configArgs, switchMappings.ToDictionary());

            var configRoot = configBuilder.Build();

            _log.Debug($"Configuration root initialized with:");
            foreach (var entry in configRoot.AsEnumerable(false))
                _log.Debug($"[{entry.Key}] = {entry.Value}");

            return configRoot;
        }
    }
}
