using DotNet.Basics.Win32;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Win32
{
    public class CommandPromptTests
    {
        [Fact]
        public void Run_ExitCode_ExitCodeIsReturned()
        {
            const int expected = 16;
            var result = CommandPrompt.Run($"robocopy");//returns 16 when no params
            result.Item1.Should().Be(expected);
        }
    }
}
