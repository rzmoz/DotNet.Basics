using System;
using System.ComponentModel;
using DotNet.Standard.Sys;
using DotNet.Standard.TestsRoot;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Standard.Tests.Sys
{
    public class ExecutableTests : TestWithHelpers
    {
        public ExecutableTests(ITestOutputHelper output) : base(output)
        { }

        [Fact]
        public void Run_ExeNotFound_ExceptionIsThrown()
        {
            ArrangeActAssertPaths(testDir =>
             {
                 Action act = () => Executable.Run(testDir.RawPath);

                 act.ShouldThrow<Win32Exception>();
             });
        }
    }
}
