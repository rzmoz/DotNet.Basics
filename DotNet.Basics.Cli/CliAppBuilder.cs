using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Cli
{
    public class CliAppBuilder : ICliAppBuilder
    {
        private readonly string[] _args;
        private readonly IDictionary<string, string> _switchMappings;
        private readonly IServiceCollection _services = new ServiceCollection();

        private Action<IServiceCollection> _configureLogging;
        private Action<IConfigurationRoot, IConfigurationBuilder> _configureConfiguration;
        private Action<IServiceCollection> _configureServices;
        private Func<IServiceCollection, IServiceProvider> _configureServiceProvider;

        public CliAppBuilder(params string[] args)
        : this(args, null)
        { }

        public CliAppBuilder(string[] args, Action<IDictionary<string, string>> switchMappings = null)
        {
            _args = args;

            _switchMappings = new Dictionary<string, string>();
            switchMappings?.Invoke(_switchMappings);
        }

        public ICliAppBuilder ConfigureLogging(Action<IServiceCollection> configureLogging)
        {
            _configureLogging = configureLogging;
            return this;
        }

        public ICliAppBuilder ConfigureAppConfiguration(Action<IConfigurationRoot, IConfigurationBuilder> configureConfiguration)
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
            var argsConfig = new ConfigurationBuilder().AddCommandLine(_args, _switchMappings).Build();

            //configure app configuration
            var appConfigBuilder = new ConfigurationBuilder();
            _configureConfiguration?.Invoke(argsConfig, appConfigBuilder);
            var appConfig = appConfigBuilder.Build();

            //configure services
            _configureServices?.Invoke(_services);

            //configure servicesProvider
            var serviceProvider = _configureServiceProvider?.Invoke(_services) ?? _services.BuildServiceProvider();
            return new CliApp(argsConfig, appConfig, serviceProvider);
        }
    }
}