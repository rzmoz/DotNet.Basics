using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.IO;
using DotNet.Basics.PowerShell;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.PowerShell
{
    public class PowerShellCliTests : TestWithHelpers
    {
        const string _greeting = @"Hello World!";
        const string _writeGreetingToHost = @"Write-Host ""Hello World!""";

        public PowerShellCliTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void RunFileInConsole_FileNotFound_ExceptionIsThrown()
        {
            var path = "SOME_PATH_THAT_DOES_NOT_EXIST.ps1";

            var errors = new List<string>();

            var exitCode = PowerShellCli.RunFileInConsole(path, writeError: error => errors.Add(error));
            exitCode.Should().Be(-196608);
            errors.Single().Should().EndWith($"The argument '{path}' to the -File parameter does not exist. Provide the path to an existing '.ps1' file as an argument to the -File parameter.");
        }

        [Fact]
        public void RunFileInConsole_ExecuteFile_ExitCodeIsCorrect()
        {
            WithTestRoot(testRoot =>
            {
                var scriptPath = testRoot.ToFile(@"PowerShell", "PsFile.ps1").FullName();

                var result = PowerShellCli.RunFileInConsole(scriptPath);

                result.Should().Be(42);
            });
        }

        [Fact]
        public void RunScript_ExecuteScript_HelloWorldIsOutputted()
        {
            var script = $@"""{_greeting}""";

            var result = PowerShellCli.RunScript(script);

            result.Single().ToString().Should().Be(_greeting);
        }

        [Fact]
        public void RunScript_WriteToHost_OutputToHostIsCaptured()
        {
            var captured = string.Empty;
            var log = new LogDispatcher();
            log.MessageLogged += (lvl, msg, e) => captured += msg;
            log.MessageLogged += (lvl, msg, e) => Output.WriteLine(msg);

            var result = PowerShellCli.RunScript(_writeGreetingToHost, log);

            captured.Should().Be(_greeting);
        }
    }
}
