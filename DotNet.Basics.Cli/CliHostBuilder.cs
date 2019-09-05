using System;
using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;
using Microsoft.Extensions.Configuration;

namespace DotNet.Basics.Cli
{
    public class CliHostBuilder
    {
        private readonly string[] _args;
        private readonly ArgsSwitchMappings _switchMappings;
        private readonly IList<Action<IConfigurationBuilder>> _customConfiguration = new List<Action<IConfigurationBuilder>>();
        private readonly IList<Action<ILogDispatcher>> _customLogging = new List<Action<ILogDispatcher>>();
        private Func<ILogDispatcher> _logDispatcherFactory;
        private readonly AppInfo _appInfo;

        public CliHostBuilder(string[] args, Action<ArgsSwitchMappings> customSwitchMappings = null, Type classThatContainStaticVoidMain = null)
        {
            _args = args;
            _switchMappings = new ArgsSwitchMappings(customSwitchMappings);
            _appInfo = new AppInfo(classThatContainStaticVoidMain);
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
        public CliHostBuilder WithConfiguration(Action<IConfigurationBuilder> add)
        {
            _customConfiguration.Add(add);
            return this;
        }

        public CliHost Build()
        {
            return BuildCustomHost((args, config, log) => new CliHost(args, config, log));
        }

        public virtual T BuildCustomHost<T>(Func<string[], IConfigurationRoot, ILogDispatcher, T> build) where T : CliHost
        {
            if (build == null) throw new ArgumentNullException(nameof(build));

            var log = InitLogging();

            log.Information($@"Initializing {_appInfo.ToString().Highlight()} with args {_args.JoinString(" ").Highlight()}");

            var configRoot = InitConfiguration(log);

            return build(_args, configRoot, log);
        }

        protected virtual ILogDispatcher InitLogging()
        {
            var log = _logDispatcherFactory?.Invoke() ?? new LogDispatcher();

            foreach (var apply in _customLogging)
                apply?.Invoke(log);
            log.Verbose($"Logger <{log.GetType().Name}> initialized");
            return log;
        }

        protected virtual IConfigurationRoot InitConfiguration(ILogDispatcher log)
        {
            if (_switchMappings.Any())
            {
                log.Debug($"Args switch mappings:");
                foreach (var entry in _switchMappings)
                    log.Debug($"{entry.Key} => {entry.Value}");
            }

            var preppedArgs = _args.CleanArgsForCli().EnsureFlagsHaveValue().EnsureEnvironmentsAreDistinct().ToArray();
            var configForEnvironments = new ConfigurationBuilder().AddCommandLine(preppedArgs, _switchMappings.ToDictionary()).Build();
            var environments = configForEnvironments.Environments();

            log.Verbose($"Environments: {environments.JoinString()}");

            //configure configuration sources
            log.Verbose($"Reading config from appSettings.json");
            var configBuilder = new ConfigurationBuilder().AddJsonFile("appSettings.json", true, false);
            foreach (var env in environments)
            {
                log.Verbose($"Reading config from appSettings.{env}.json");
                configBuilder.AddJsonFile($"appSettings.{env}.json", true, false);
            }

            if (_customConfiguration.Any())
                log.Verbose($"Reading config from custom configuration in host");
            foreach (var configurationAction in _customConfiguration)
                configurationAction?.Invoke(configBuilder);

            if (_args.Any())
                log.Verbose($"Reading config from args");

            configBuilder.AddCommandLine(preppedArgs, _switchMappings.ToDictionary());

            var configRoot = configBuilder.Build();

            log.Verbose($"Configuration root initialized with:");
            foreach (var entry in configRoot.AsEnumerable(false))
                log.Verbose($"  [\"{entry.Key}\"] = {entry.Value}");

            return configRoot;
        }
    }
}
