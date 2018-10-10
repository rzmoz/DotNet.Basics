using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Cli
{
    public class CliAppBuilder : ICliAppBuilder
    {
        private readonly string[] _args;
        private readonly IServiceCollection _services = new ServiceCollection();

        private Action<IServiceCollection> _configureLogging;
        private Func<IDictionary<string, string>> _switchMappings;
        private Action<IConfigurationBuilder> _configureConfiguration;
        private Action<IServiceCollection> _configureServices;
        private Func<IServiceCollection, IServiceProvider> _configureServiceProvider;

        public CliAppBuilder(params string[] args)
        {
            _args = args;
        }

        public ICliAppBuilder ConfigureLogging(Action<IServiceCollection> configureLogging)
        {
            _configureLogging = configureLogging;
            return this;
        }

        public ICliAppBuilder ConfigureCliSwitchMappings(Func<IDictionary<string, string>> switchMappings)
        {
            _switchMappings = switchMappings;
            return this;
        }

        public ICliAppBuilder ConfigureAppConfiguration(Action<IConfigurationBuilder> configureConfiguration)
        {
            _configureConfiguration = configureConfiguration;
            return this;
        }

        public ICliAppBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            _configureServices = configureServices;
            return this;
        }

        public ICliAppBuilder ConfigureServiceProvider(Func<IServiceCollection, IServiceProvider> configureServiceProvider)
        {
            _configureServiceProvider = configureServiceProvider;
            return this;
        }

        public ICliApp Build()
        {
            //configure logging
            _configureLogging?.Invoke(_services);

            //configure cli args config
            var switchMappings = _switchMappings?.Invoke();
            var argsConfig = (switchMappings == null ? new ConfigurationBuilder().AddCommandLine(_args) : new ConfigurationBuilder().AddCommandLine(_args, switchMappings)).Build();


            //configure app configuration
            var appConfigBuilder = new ConfigurationBuilder();
            _configureConfiguration?.Invoke(appConfigBuilder);
            var appConfig = appConfigBuilder.Build();

            //configure services
            _configureServices?.Invoke(_services);

            //configure servicesProvider
            var serviceProvider = _configureServiceProvider?.Invoke(_services) ?? _services.BuildServiceProvider();
            return new CliApp(argsConfig, appConfig, serviceProvider);
        }
    }
}