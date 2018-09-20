using System.Linq;
using DotNet.Basics.PowerShell;
using DotNet.Basics.TestsRoot;
using DotNet.Basics.IO;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.PowerShell
{
    public class PowerShellCliTests : TestWithHelpers
    {
        public PowerShellCliTests(ITestOutputHelper output) : base(output)
        {
        }
        
        [Fact]
        public void RunScript_ExecuteScript_HelloWorldIsOutputted()
        {
            const string greeting = @"Hello World!";

            var result = PowerShellCli.RunScript($"\"{greeting}\"");

            result.Single().ToString().Should().Be(greeting);
        }

        [Fact]
        public void RunScript_WriteToHost_OutputToHostIsCaptured()
        {
            const string greeting = "Hello world!";

            var result = PowerShellCli.RunScript($"Write-Host \"{greeting}\"");
        }

        [Fact]
        public void RemoveItem_DeleteFilesAndFolders_DirIsEmptied()
        {
            ArrangeActAssertPaths(dir =>
            {
                dir.CreateSubDir("myDir");
                dir.ToFile("myFile.txt").WriteAllText("nothing");

                dir.EnumerateDirectories().Count().Should().Be(1);
                dir.EnumerateFiles().Count().Should().Be(1);
                dir.EnumeratePaths().Count().Should().Be(2);

                //act
                PowerShellCli.RemoveItem($@"{dir.FullName()}\*");

                //assert
                dir.EnumerateDirectories().Count().Should().Be(0);
                dir.EnumerateFiles().Count().Should().Be(0);
                dir.EnumeratePaths().Count().Should().Be(0);
            });

        }
    }
}
