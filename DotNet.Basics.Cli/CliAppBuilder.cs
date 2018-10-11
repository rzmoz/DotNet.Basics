using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Cli
{
    public class CliAppBuilder : ICliAppBuilder
    {
        private readonly IServiceCollection _services = new ServiceCollection();

        private Action<IServiceCollection> _configureLogging;
        private Action<IConfigurationBuilder> _configureConfiguration;
        private Action<IServiceCollection> _configureServices;
        private Func<IServiceCollection, IServiceProvider> _configureServiceProvider;

        public ICliAppBuilder ConfigureLogging(Action<IServiceCollection> configureLogging)
        {
            _configureLogging = configureLogging;
            return this;
        }

        public ICliAppBuilder ConfigureConfiguration(Action<IConfigurationBuilder> configureConfiguration)
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

            //configure app configuration
            var appConfigBuilder = new ConfigurationBuilder();
            _configureConfiguration?.Invoke(appConfigBuilder);
            var appConfig = appConfigBuilder.Build();

            //configure services
            _configureServices?.Invoke(_services);

            //configure servicesProvider
            var serviceProvider = _configureServiceProvider?.Invoke(_services) ?? _services.BuildServiceProvider();
            return new CliApp(appConfig, serviceProvider);
        }
    }
}