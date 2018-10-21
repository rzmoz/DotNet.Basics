using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Cli
{
    public interface ICliAppBuilder
    {
        ICliAppBuilder ConfigureLogging(Action<IServiceCollection> configureServices);
        ICliAppBuilder ConfigureConfiguration(Action<IConfigurationBuilder> configureConfiguration);
        ICliAppBuilder ConfigureServices(Action<IServiceCollection> configureServices);
        ICliAppBuilder CreateServiceProvider(Func<IServiceCollection, IServiceProvider> configureServiceProvider);

        ICliApp Build();
    }
}
