using System;
using System.Linq;
using System.Management.Automation;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Sys
{
    [TestFixture]
    public class PowerShellConsoleTests
    {
        [Test]
        public void RunCommand_ExceptionsBubble_ExcpetionIsThrown()
        {
            var cmdlet = new PowerShellCmdlet("Copy-Item").AddParameter("asdasdasd").AddParameter("asdasdasdad");//invalid parameters

            Action action = () => cmdlet.Run();

            action.ShouldThrow<ParameterBindingException>();
        }

        [Test]
        public void RunScript_ExecuteScript_HelloworldIsOutputted()
        {
            const string greeting = @"Hello World!";

            var result = PowerShellConsole.RunScript($"\"{greeting}\"");

            result.Single().ToString().Should().Be(greeting);
        }
    }
}
