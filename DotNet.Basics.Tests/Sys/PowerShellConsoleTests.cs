using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Sys
{
    [TestFixture]
    public class PowerShellConsoleTests
    {
        [Test]
        public void RunFunction_ExecuteScript_HelloworldIsOutputted()
        {
            const string greetee = "Wooorld";

            var result = PowerShellConsole.RunFunction("Greet", new KeyValuePair<string, object>("greetee", greetee), @".\Greetings.ps1");

            result.PassThru.Single().ToString().Should().Be($"Hello {greetee}!");
        }

        [Test]
        public void RunScript_ExecuteScript_HelloworldIsOutputted()
        {
            const string greeting = @"Hello World!";

            var result = PowerShellConsole.RunScript($"\"{greeting}\"");

            result.PassThru.Single().ToString().Should().Be(greeting);
        }
    }
}
