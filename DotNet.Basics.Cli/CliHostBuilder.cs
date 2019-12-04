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
        private readonly string[] _cliArgs;
        private readonly IList<Action<IConfigurationBuilder>> _customConfiguration = new List<Action<IConfigurationBuilder>>();
        private readonly IList<Action<ILogger>> _customLogging = new List<Action<ILogger>>();
        private Func<ILogger> _logDispatcherFactory;
        private readonly AppInfo _appInfo;
        private readonly bool _verboseIsSet;

        public CliHostBuilder(string[] cliArgs, Type classThatContainStaticVoidMain = null)
        {
            _cliArgs = cliArgs;
            _verboseIsSet = _cliArgs.IsSet("verbose", false);
            _appInfo = new AppInfo(classThatContainStaticVoidMain);
        }
        public CliHostBuilder SetLogDispatcherFactory(Func<ILogger> logDispatcherFactory)
        {
            _logDispatcherFactory = logDispatcherFactory;
            return this;
        }
        public CliHostBuilder WithLogging(Action<ILogger> configureLogging)
        {
            _customLogging.Add(configureLogging);
            return this;
        }
        public CliHostBuilder WithConfiguration(Action<IConfigurationBuilder> configureConfiguration)
        {
            _customConfiguration.Add(configureConfiguration);
            return this;
        }

        public CliHost Build(Action<ArgsSwitchMappings> customSwitchMappings = null)
        {
            return BuildCustomHost((config, log) => new CliHost(_cliArgs, config, log), customSwitchMappings);
        }
        public virtual THost BuildCustomHost<THost>(Func<IConfigurationRoot, ILogger, THost> build, Action<ArgsSwitchMappings> customSwitchMappings = null)
        {
            if (build == null) throw new ArgumentNullException(nameof(build));

            var log = InitLogging();

            var appInfo = $@"Initializing {_appInfo.ToString().Highlight()}";
            if (_verboseIsSet)
                appInfo += $@" with args {_cliArgs.JoinString(" ").Highlight()}";
            log.Info(appInfo);
            var rawArgs = InitRawArgs(_cliArgs);
            var configRoot = InitConfiguration(rawArgs, log, customSwitchMappings);
            return build(configRoot, log);
        }

        protected virtual ILogger InitLogging()
        {
            var log = _logDispatcherFactory?.Invoke() ?? new Logger();
            if (_verboseIsSet)
                log.Verbose($"Log Dispatcher <{log.GetType().Name}> initialized");

            foreach (var apply in _customLogging)
                apply?.Invoke(log);

            if (log.HasListeners == false)
                log.AddFirstSupportedConsole();

            return log;
        }

        protected virtual string[] InitRawArgs(string[] args)
        {
            return args.CleanArgsForCli()
                       .EnsureFlagsHaveValue()
                       .EnsureEnvironmentsAreDistinct()
                       .ToArray();
        }

        protected virtual IConfigurationRoot InitConfiguration(string[] args, ILogger log, Action<ArgsSwitchMappings> customSwitchMappings)
        {

            var switchMappings = new ArgsSwitchMappings(mappings =>
            {
                mappings.Add("env", ArgsExtensions.EnvironmentsKey);
                mappings.Add("envs", ArgsExtensions.EnvironmentsKey);
                mappings.Add("environment", ArgsExtensions.EnvironmentsKey);
            });
            customSwitchMappings?.Invoke(switchMappings);

            if (_verboseIsSet)
            {
                log.Verbose($"Args aliases:");
                foreach (var entry in switchMappings)
                    log.Verbose($"{entry.Key} => {ArgsExtensions.MicrosoftExtensionsArgsSwitch}{entry.Value}");
            }

            var configForEnvironments = new ConfigurationBuilder().AddCommandLine(args, switchMappings.ToDictionary()).Build();
            var environments = configForEnvironments.Environments();

            if (_verboseIsSet)
                log.Verbose($"Environments: {environments.JoinString().Highlight()}");

            //configure configuration sources
            if (_verboseIsSet)
                log.Verbose($"Reading config from appSettings.json");

            //add default config file
            _customConfiguration.Add(config => config.AddJsonFile("appSettings.json", true, false));
            foreach (var env in environments)//add environment specific configs
                _customConfiguration.Add(config => config.AddJsonFile($"appSettings.{env}.json", true, false));

            var configBuilder = new ConfigurationBuilder();
            foreach (var configurationAction in _customConfiguration)
                configurationAction?.Invoke(configBuilder);

            if (_verboseIsSet && _cliArgs.Any())
                log.Verbose("Reading config from args");
            configBuilder.AddCommandLine(args, switchMappings.ToDictionary());

            var configRoot = configBuilder.Build();

            if (_verboseIsSet)
            {
                log.Verbose($"Configuration root initialized with:");
                foreach (var entry in configRoot.AsEnumerable(false))
                    log.Verbose($"  [\"{entry.Key}\"] = {entry.Value}");
            }

            return configRoot;
        }
    }
}