using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.PowerShell;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.PowerShell
{
    public class PowerShellCliTests : TestWithHelpers
    {
        const string _greeting = @"Hello World!";
        const string _writeGreetingToHost = @"Write-Host ""Hello World!""";

        public PowerShellCliTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void RunScript_ExecuteScript_HelloWorldIsOutputted()
        {
            var script = $@"""{_greeting}""";

            var result = PowerShellCli.Run(new[] {script});

            result.Single().ToString().Should().Be(_greeting);
        }

        [Fact]
        public void RunScript_WriteToHost_OutputToHostIsCaptured()
        {
            var captured = string.Empty;
            var log = new LogDispatcher();
            log.MessageLogged += (lvl, msg, e) => captured += msg;
            log.MessageLogged += (lvl, msg, e) => Output.WriteLine(msg);

            var result = PowerShellCli.Run(log, _writeGreetingToHost);

            captured.Should().Be(_greeting);
        }
    }
}
