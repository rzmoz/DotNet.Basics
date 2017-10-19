using System.Linq;
using DotNet.Basics.IO;
using DotNet.Basics.PowerShell;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.PowerShell
{
    public class PowerShellConsoleTests
    {
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

            result.Single().Should().Be(greeting);
        }

        [Fact]
        public void RemoveItem_DeleteFilesAndFolders_DirIsEmptied()
        {
            var dir = TestRoot.Dir.Add("RemoveItem_DeleteFilesAndFolders_DirIsEmptied");
            dir.CreateSubDir("myDir");
            dir.ToFile("myFile.txt").WriteAllText("nothing");

            dir.EnumerateDirectories().Count().Should().Be(1);
            dir.EnumerateFiles().Count().Should().Be(1);
            dir.EnumeratePaths().Count().Should().Be(2);

            //act
            PowerShellCli.RemoveItem($"{dir.FullPath()}\\*");

            //assert
            dir.EnumerateDirectories().Count().Should().Be(0);
            dir.EnumerateFiles().Count().Should().Be(0);
            dir.EnumeratePaths().Count().Should().Be(0);
        }
    }
}
