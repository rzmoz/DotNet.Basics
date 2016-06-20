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

            var consolePath = TestContext.CurrentContext.TestDirectory.ToFile("DotNet.Basics.Tests.Console.exe");

            var observedExitCode = CommandPrompt.Run($"{consolePath } {exitCode}");
            observedExitCode.Should().Be(exitCode);
        }
    }
}
