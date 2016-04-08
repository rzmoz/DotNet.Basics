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
            var observedExitCode = CommandPrompt.Run($".\\DotNet.Basics.Tests.Console.exe {exitCode}");
            observedExitCode.Should().Be(exitCode);
        }
    }
}
