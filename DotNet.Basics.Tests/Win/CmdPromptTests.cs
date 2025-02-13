using DotNet.Basics.Win;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.Win
{
    public class CmdPromptTests(ITestOutputHelper output)
    {
        [Fact]
        public void Run_ExitCode_ExitCodeIsReturned()
        {
            const int expected = 16;
            var exitCode = CmdPrompt.Run($"robocopy");//returns 16 when no params
            output.WriteLine($"result.Output");
            exitCode.Should().Be(expected);
        }
    }
}
