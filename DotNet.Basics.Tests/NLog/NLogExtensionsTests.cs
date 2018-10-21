using DotNet.Basics.NLog;
using DotNet.Basics.Sys;
using DotNet.Basics.Tests.NetCore.EchoOut;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Sdk;

namespace DotNet.Basics.Tests.NLog
{
    public class NLogExtensionsTests
    {


        [Fact]
        public void AddNLogging_AddLogging_LoggingIsAdded()
        {
            var services = new ServiceCollection();

            //act
            services.AddNLogging(config => config.AddColoredConsoleTarget());

            //assert
            var provider = services.BuildServiceProvider();
            var log = provider.GetService<ILogger>();
            log.Should().BeOfType<Logger<ILogger>>();
        }
        [Fact(Skip = "too difficult to call echoOut")]
        public void AddNLogging_Configuration_LoggingIsInOutput()
        {
            var input = 947;

            var dotNetPath = "C:\\Program Files\\dotnet\\dotnet.exe";
            var echoOutPath = $"{typeof(EchoOutProgram).Assembly.Location}";
            //act
            var result = ExternalProcess.Run($"\"{dotNetPath}\"", $" \"{echoOutPath}\" {input}");

            result.ExitCode.Should().Be(input);
            result.Output.Should().Contain($"Starting {typeof(EchoOutProgram).Namespace}...");
            result.Output.Should().Contain($"{input}");
        }
    }
}
