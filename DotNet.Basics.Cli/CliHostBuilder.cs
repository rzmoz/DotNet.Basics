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
        private readonly string[] _rawArgs;
        private readonly ArgsSwitchMappings _switchMappings;
        private readonly IList<Action<IConfigurationBuilder>> _customConfiguration = new List<Action<IConfigurationBuilder>>();
        private readonly IList<Action<ILogDispatcher>> _customLogging = new List<Action<ILogDispatcher>>();
        private Func<ILogDispatcher> _logDispatcherFactory;
        private readonly AppInfo _appInfo;
        private readonly bool _verboseIsSet;
        private Func<ICliConfiguration, ILogDispatcher, ArgsHydrateFromConfigVisitor> _argsHydrateVisitor = null;

        public CliHostBuilder(string[] rawArgs, Type classThatContainStaticVoidMain = null)
        {
            _rawArgs = rawArgs;
            _verboseIsSet = _rawArgs.IsSet("verbose", false);
            _switchMappings = new ArgsSwitchMappings();
            _appInfo = new AppInfo(classThatContainStaticVoidMain);
        }

        public CliHostBuilder WithArgsHydrator(Func<ICliConfiguration, ILogDispatcher, ArgsHydrateFromConfigVisitor> getArgsHydrateVisitor)
        {
            _argsHydrateVisitor = getArgsHydrateVisitor;
            return this;
        }

        public CliHostBuilder WithArgsSwitchMappings(Action<ArgsSwitchMappings> customSwitchMappings = null)
        {
            customSwitchMappings?.Invoke(_switchMappings);
            return this;
        }

        public CliHostBuilder SetLogDispatcherFactory(Func<ILogDispatcher> logDispatcherFactory)
        {
            _logDispatcherFactory = logDispatcherFactory;
            return this;
        }
        public CliHostBuilder WithLogging(Action<ILogDispatcher> configureLogging)
        {
            _customLogging.Add(configureLogging);
            return this;
        }
        public CliHostBuilder WithConfiguration(Action<IConfigurationBuilder> configureConfiguration)
        {
            _customConfiguration.Add(configureConfiguration);
            return this;
        }

        public CliHost Build()
        {
            return BuildCustomHost<CliHost, string[]>((config, log) => new CliHost(_rawArgs, config, log));
        }
        public CliHost<T> Build<T>(Func<IConfigurationRoot, ILogDispatcher, T> buildArgs)
        {
            return BuildCustomHost<CliHost<T>, T>((config, log) =>
             {
                 var args = buildArgs(config, log);
                 return new CliHost<T>(args, _rawArgs, config, log);
             });
        }

        public virtual T BuildCustomHost<T>(Func<IConfigurationRoot, ILogDispatcher, T> build) where T : ICliHost<string[]>
        {
            return BuildCustomHost<T, string[]>(build);
        }
        public virtual T BuildCustomHost<T, TK>(Func<IConfigurationRoot, ILogDispatcher, T> build) where T : ICliHost<TK>
        {
            if (build == null) throw new ArgumentNullException(nameof(build));

            var log = InitLogging();

            var appInfo = $@"Initializing {_appInfo.ToString().Highlight()}";
            if (_verboseIsSet)
                appInfo += $@" with args {_rawArgs.JoinString(" ").Highlight()}";
            log.Info(appInfo);
            var args = InitArgs(_rawArgs);
            var configRoot = InitConfiguration(args, log);


            var host = build(configRoot, log);
            if (_argsHydrateVisitor != null)
            {
                var argsHydrator = _argsHydrateVisitor.Invoke(host, log);
                argsHydrator.HydrateFromConfig(host.Args);
            }

            return host;
        }

        protected virtual ILogDispatcher InitLogging()
        {
            var log = _logDispatcherFactory?.Invoke() ?? new LogDispatcher();
            if (_verboseIsSet)
                log.Verbose($"Log Dispatcher <{log.GetType().Name}> initialized");

            foreach (var apply in _customLogging)
                apply?.Invoke(log);

            if (log.HasListeners == false)
                log.AddFirstSupportedConsole();

            return log;
        }

        protected virtual string[] InitArgs(string[] args)
        {
            return args.CleanArgsForCli()
                       .EnsureFlagsHaveValue()
                       .EnsureEnvironmentsAreDistinct()
                       .ToArray();
        }

        protected virtual IConfigurationRoot InitConfiguration(string[] args, ILogDispatcher log)
        {
            if (_verboseIsSet && _switchMappings.Any())
            {
                log.Verbose($"Args aliases:");
                foreach (var entry in _switchMappings)
                    log.Verbose($"{entry.Key} => {ArgsExtensions.MicrosoftExtensionsArgsSwitch}{entry.Value}");
            }

            var configForEnvironments = new ConfigurationBuilder().AddCommandLine(args, _switchMappings.ToDictionary()).Build();
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

            if (_verboseIsSet && _rawArgs.Any())
                log.Verbose("Reading config from args");
            configBuilder.AddCommandLine(args, _switchMappings.ToDictionary());

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