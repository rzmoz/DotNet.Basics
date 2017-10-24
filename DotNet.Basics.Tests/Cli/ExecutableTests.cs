using System;
using System.ComponentModel;
using DotNet.Basics.Cli;
using DotNet.Basics.IO;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Cli
{
    public class ExecutableTests
    {
        [Fact]
        public void Run_ExeNotFound_ExceptionIsThrown()
        {
            var fullPath = TestRoot.Dir.Add("Run_ExeNotFound_ExceptionIsThrown").RawPath;

            Action action = () => Executable.Run(fullPath);

            action.ShouldThrow<Win32Exception>();
        }
    }
}
