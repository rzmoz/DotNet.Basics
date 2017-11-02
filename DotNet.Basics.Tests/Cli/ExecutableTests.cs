using System;
using System.ComponentModel;
using DotNet.Basics.Cli;
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
            ArrangeActAssertPaths(testDir =>
             {
                 Action action = () => Executable.Run(testDir.RawPath);

                 action.ShouldThrow<Win32Exception>();
             });
        }
    }
}
