using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Sys
{
    [TestFixture]
    public class CommandPromptTests
    {
        [Test]
        [TestCase(1)]
        [TestCase(0)]
        public void Run_ExitCode_ExitCodeIsReturned(int exitCode)
        {
            var exitCodeString = "";
            if (exitCode != 0)
                exitCodeString = "-ExitCode " + exitCode;

            var observedExitCode = CommandPrompt.Run(@".\CSharp.Basics.Tests.Console.exe " + exitCodeString);
            observedExitCode.Should().Be(exitCode);
        }
    }
}
