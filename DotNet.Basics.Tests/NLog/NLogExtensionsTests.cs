using DotNet.Basics.NLog;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DotNet.Basics.Tests.NLog
{
    public class NLogExtensionsTests
    {
        [Fact]
        public void AddNLogging_AddLogging_LoggingIsAdded()
        {
            var services = new ServiceCollection();

            //act
            services.AddNLogging(config => config.AddColoredConsoleTargetWithDotNetBasicsDefaultSettings());

            //assert
            var provider = services.BuildServiceProvider();
            var log = provider.GetService<ILogger>();
            log.Should().BeOfType<Logger<ILogger>>();
        }
    }
}
