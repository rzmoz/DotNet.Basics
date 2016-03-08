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
            var observedExitCode = CommandPrompt.Run($".\\DotNet.Basics.Tests.Console.exe {exitCode}");
            observedExitCode.Should().Be(exitCode);
        }
    }
}
