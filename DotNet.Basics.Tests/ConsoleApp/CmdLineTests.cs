using DotNet.Basics.ConsoleApp;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.ConsoleApp
{
    [TestFixture]
    public class CmdLineTests
    {
        [Test]
        [TestCase("-")]//dash
        [TestCase("/")]//slash
        public void Parse_ParamNoValue_ParamExists(string paramIndicator)
        {
            const string paramName = "MyParam";

            var cmdLine = new CmdLine().Register(paramName, Required.Yes, AllowEmpty.Yes, param =>
            {
                param.Exists.Should().BeTrue();
            });

            var parseResult = cmdLine.Parse(new[] { $"{paramIndicator}{paramName}" });
            parseResult.Should().BeTrue();
        }
    }
}
