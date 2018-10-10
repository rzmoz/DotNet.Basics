using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Cli
{
    public interface ICliAppBuilder
    {
        ICliAppBuilder ConfigureLogging(Action<IServiceCollection> configureServices);
        ICliAppBuilder ConfigureCliSwitchMappings(Func<IDictionary<string, string>> switchMappings);
        ICliAppBuilder ConfigureAppConfiguration(Action<IConfigurationBuilder> configureConfiguration);
        ICliAppBuilder ConfigureServices(Action<IServiceCollection> configureServices);
        ICliAppBuilder ConfigureServiceProvider(Func<IServiceCollection, IServiceProvider> configureServiceProvider);

        ICliApp Build();
    }
}
