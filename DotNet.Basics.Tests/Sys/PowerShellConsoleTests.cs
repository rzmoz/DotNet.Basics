using System.Linq;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class PowerShellConsoleTests
    {
        [Fact]
        public void RunScript_ExecuteScript_HelloWorldIsOutputted()
        {
            const string greeting = @"Hello World!";

            var result = PowerShellConsole.RunScript($"\"{greeting}\"");

            result.Single().ToString().Should().Be(greeting);
        }

        [Fact]
        public void RunScript_WriteToHost_OutputToHostIsCaptured()
        {
            const string greeting = "Hello world!";

            var result = PowerShellConsole.RunScript($"Write-Host \"{greeting}\"");
        }

        [Fact]
        public void RemoveItem_DeleteFilesAndFolders_DirIsEmptied()
        {
            var dir = @"RemoveItem_DeleteFilesAndFolders_DirIsEmptied".ToDir();
            dir.CreateSubDir("myDir");
            "nothing".WriteAllText(dir.ToFile("myFile.txt"));

            dir.EnumerateDirectories().Count().Should().Be(1);
            dir.EnumerateFiles().Count().Should().Be(1);
            dir.EnumeratePaths().Count().Should().Be(2);

            //act
            PowerShellConsole.RemoveItem($"{dir.FullName}\\*");

            //assert
            dir.EnumerateDirectories().Count().Should().Be(0);
            dir.EnumerateFiles().Count().Should().Be(0);
            dir.EnumeratePaths().Count().Should().Be(0);
        }
    }
}
