﻿using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.Sys
{
    public class CmdPromptTests
    {
        private readonly ITestOutputHelper _output;

        public CmdPromptTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public void Run_ExitCode_ExitCodeIsReturned()
        {
            const int expected = 16;
            var result = CliPrompt.Run($"robocopy");//returns 16 when no params
            _output.WriteLine($"result.Output");
            result.ExitCode.Should().Be(expected);
        }
    }
}
