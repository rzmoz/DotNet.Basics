using System;
using System.ComponentModel;
using DotNet.Basics.Cli;
using DotNet.Basics.Sys;
using DotNet.Basics.TestsRoot;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.Cli
{
    public class ExecutableTests : TestWithHelpers
    {
        public ExecutableTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Run_ExeNotFound_ExceptionIsThrown()
        {
            var fullPath = TestRoot.ToDir("Run_ExeNotFound_ExceptionIsThrown");

            Action action = () => Executable.Run(fullPath.RawPath);

            action.ShouldThrow<Win32Exception>();
        }
    }
}
