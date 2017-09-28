using DotNet.Basics.Shell;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Shell
{
    public class CmdPromptTests
    {
        [Fact]
        public void Run_ExitCode_ExitCodeIsReturned()
        {
            const int expected = 16;
            var result = CmdPrompt.Run($"robocopy");//returns 16 when no params
            result.Item1.Should().Be(expected);
        }
    }
}
