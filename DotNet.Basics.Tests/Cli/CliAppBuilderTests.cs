using System.Collections.Generic;
using Autofac.Extensions.DependencyInjection;
using DotNet.Basics.Autofac;
using DotNet.Basics.Cli;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotNet.Basics.Tests.Cli
{
    public class CliAppBuilderTests
    {
        private const string _argKey = "Path";
        private const string _argValue = "MyPath";

        [Fact]
        public void Ctor_CliArgsConfiguration_ArgsConfigAreConfiguredInApp()
        {
            var app = new CliAppBuilder($"{_argKey}={_argValue}").Build();

            app.CliArgsConfiguration[_argKey].Should().Be(_argValue);
        }

        [Fact]
        public void ConfigureCliSwitchMappings_CliArgsConfiguration_ArgsConfigAreConfiguredInApp()
        {
            var shortSwitch = $"-{_argKey[0]}";

            var app = new CliAppBuilder($"{shortSwitch}={_argValue}")
                .ConfigureCliSwitchMappings(() => new Dictionary<string, string> { { shortSwitch, _argKey } })
                .Build();
            app.CliArgsConfiguration[_argKey].Should().Be(_argValue);
        }

        [Fact]
        public void ConfigureLogging_AddService_ServiceIsAdded()
        {
            var type = typeof(CliAppBuilderTests);

            var app = new CliAppBuilder().ConfigureLogging(services =>
                {
                    services.AddTransient(type);
                }).Build();

            app.ServiceProvider.GetService(type).Should().BeOfType(type);
        }
        [Fact]
        public void ConfigureServices_AddService_ServiceIsAdded()
        {
            var type = typeof(CliAppBuilderTests);

            var app = new CliAppBuilder().ConfigureServices(services =>
            {
                services.AddTransient(type);
            }).Build();

            app.ServiceProvider.GetService(type).Should().BeOfType(type);
        }

        [Fact]
        public void ConfigureServiceProvider_ServiceProviderIsCustom_ServiceProviderIsOverridden()
        {
            var appWithDefaultServiceProvider = new CliAppBuilder().Build();
            appWithDefaultServiceProvider.ServiceProvider.Should().BeOfType<ServiceProvider>();

            var appWithCustomServiceProvider = new CliAppBuilder().ConfigureServiceProvider(services => new AutofacBuilder().ServiceProvider).Build();
            appWithCustomServiceProvider.ServiceProvider.Should().BeOfType<AutofacServiceProvider>();
        }

        [Fact]
        public void ConfigureAppConfiguration_AppConfiguration_AppConfigIsSet()
        {
            var app = new CliAppBuilder().ConfigureAppConfiguration(config =>
                {
                    config.AddInMemoryCollection(new[] { new KeyValuePair<string, string>(_argKey, _argValue) });
                }).Build();

            app.AppConfiguration[_argKey].Should().Be(_argValue);
        }

        [Fact]
        public void Build_InvocationAndOrder_AllMethodsAreInvoked()
        {
            var builder = new CliAppBuilder(new string[0] { });

            //arrange

            var loggingConfigured = false;
            var switchMappingsConfigured = false;
            var configurationConfigured = false;
            var servicesConfigured = false;
            var serviceProviderConfigured = false;

            builder.ConfigureLogging(services =>
            {
                loggingConfigured.Should().BeFalse();
                loggingConfigured = true;
            });
            builder.ConfigureCliSwitchMappings(() =>
            {
                loggingConfigured.Should().BeTrue();
                switchMappingsConfigured.Should().BeFalse();
                switchMappingsConfigured = true;
                return null;
            });
            builder.ConfigureAppConfiguration(config =>
            {
                switchMappingsConfigured.Should().BeTrue();
                configurationConfigured.Should().BeFalse();
                configurationConfigured = true;
            });
            builder.ConfigureServices(services =>
            {
                configurationConfigured.Should().BeTrue();
                servicesConfigured.Should().BeFalse();
                servicesConfigured = true;
            });
            builder.ConfigureServiceProvider(services =>
            {
                servicesConfigured.Should().BeTrue();
                serviceProviderConfigured = true;
                return null;
            });

            loggingConfigured.Should().BeFalse();
            switchMappingsConfigured.Should().BeFalse();
            configurationConfigured.Should().BeFalse();
            servicesConfigured.Should().BeFalse();
            serviceProviderConfigured.Should().BeFalse();

            //act
            builder.Build();

            //assert
            loggingConfigured.Should().BeTrue();
            switchMappingsConfigured.Should().BeTrue();
            configurationConfigured.Should().BeTrue();
            servicesConfigured.Should().BeTrue();
            serviceProviderConfigured.Should().BeTrue();
        }
    }
}
