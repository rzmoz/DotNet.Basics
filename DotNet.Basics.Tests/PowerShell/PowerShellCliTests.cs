using System.Linq;
using DotNet.Basics.PowerShell;

using DotNet.Basics.IO;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.PowerShell
{
    public class PowerShellCliTests : TestWithHelpers
    {
        public PowerShellCliTests(ITestOutputHelper output) : base(output)
        {
        }
        
        [Fact]
        public void RunScript_ExecuteScript_HelloWorldIsOutputted()
        {
            const string greeting = @"Hello World!";

            var result = PowerShellCli.RunScript($"\"{greeting}\"");

            result.Single().ToString().Should().Be(greeting);
        }

        [Fact]
        public void RunScript_WriteToHost_OutputToHostIsCaptured()
        {
            const string greeting = "Hello world!";

            var result = PowerShellCli.RunScript($"Write-Host \"{greeting}\"");
        }
    }
}
