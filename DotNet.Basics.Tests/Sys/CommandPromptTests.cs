using System;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    
    public class CommandPromptTests
    {
        [Fact]
        public void Run_ExitCode_ExitCodeIsReturned()
        {
            const int exitCode = 16;
            
            var consolePath = @"DotNet.Basics.TestsConsole.exe".ToFile();

            var observedExitCode = CommandPrompt.Run($"{consolePath } {exitCode}");
            observedExitCode.Should().Be(exitCode);
        }
    }
}
