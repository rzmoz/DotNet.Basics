﻿using System;
using System.ComponentModel;
using System.IO;
using DotNet.Basics.Sys;

using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.Sys
{
    public class ExternalProcessTests : TestWithHelpers
    {
        public ExternalProcessTests(ITestOutputHelper output) : base(output)
        { }

        [Fact]
        public void Run_ExeNotFound_ExceptionIsThrown()
        {
            ArrangeActAssertPaths(testDir =>
             {
                 Action act = () => ExternalProcess.Run(testDir.RawPath);

                 act.Should().Throw<Win32Exception>();
             });
        }
    }
}
