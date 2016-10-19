using System;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Sys
{
    [TestFixture]
    public class CommandPromptTests
    {
        [Test]
        public void Run_ExitCode_ExitCodeIsReturned()
        {
            const int exitCode = 16;

            CommandPrompt.StandardOut += Console.WriteLine;

            var consolePath = TestContext.CurrentContext.TestDirectory.ToFile("DotNet.Basics.TestsConsole.exe");

            var observedExitCode = CommandPrompt.Run($"{consolePath } {exitCode}");
            observedExitCode.Should().Be(exitCode);
        }
    }
}
