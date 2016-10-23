using System;
using System.Linq;
using System.Management.Automation;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Sys
{
    [TestFixture]
    public class PowerShellConsoleTests
    {
        [Test]
        public void RunScript_ExecuteScript_HelloWorldIsOutputted()
        {
            const string greeting = @"Hello World!";

            var result = PowerShellConsole.RunScript($"\"{greeting}\"");

            result.Single().ToString().Should().Be(greeting);
        }

        [Test]
        public void RunScript_WriteToHost_OutputToHostIsCaptured()
        {
            const string greeting = "Hello world!";

            var result = PowerShellConsole.RunScript($"Write-Host \"{greeting}\"");
        }

        [Test]
        public void RemoveItem_DeleteFilesAndFolders_DirIsEmptied()
        {
            var dir = TestContext.CurrentContext.TestDirectory.ToDir("RemoveItem_DeleteFilesAndFolders_DirIsEmptied");
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
