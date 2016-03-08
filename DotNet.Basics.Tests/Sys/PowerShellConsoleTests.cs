using System.Collections.Generic;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Sys
{
    [TestFixture]
    public class PowerShellConsoleTests
    {
        private PowerShellConsole _psc;

        [SetUp]
        public void SetUp()
        {
            _psc = new PowerShellConsole();
        }

        [Test]
        public void RunFunction_ExecuteScript_HelloworldIsOutputted()
        {
            const string greetee = "Wooorld";

            var result = _psc.RunFunction("Greet", new KeyValuePair<string, object>("greetee", greetee), @".\Greetings.ps1");

            result.Should().Be($"Hello {greetee}!");
        }

        [Test]
        public void RunScript_ExecuteScript_HelloworldIsOutputted()
        {
            const string greeting = @"Hello World!";

            var result = _psc.RunScript($"\"{greeting}\"");

            result.Should().Be(greeting);
        }
    }
}
