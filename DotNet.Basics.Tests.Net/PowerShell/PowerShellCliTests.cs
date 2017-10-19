using System.IO;
using System.Linq;
using DotNet.Basics.PowerShell;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Net.PowerShell
{
    public class PowerShellCliTests
    {
        [Fact]
        public void RunScript_ExecuteScript_HelloWorldIsOutputted()
        {
            const string greeting = @"Hello World!";

            var result = PowerShellCli.RunScript($"\"{greeting}\"");

            result.Single().ToString().Should().Be(greeting);
        }
        
        [Fact]
        public void RemoveItem_DeleteFilesAndFolders_DirIsEmptied()
        {
            var dir = new DirectoryInfo($@"{TestRoot.Dir}\RemoveItem_DeleteFilesAndFolders_DirIsEmptied");

            //create subdir
            Directory.CreateDirectory($@"{dir}\myDir");
            
            File.WriteAllText($@"{dir}\myFile.txt","nonthing");
            
            dir.EnumerateDirectories().Count().Should().Be(1);
            dir.EnumerateFiles().Count().Should().Be(1);
            dir.EnumerateFileSystemInfos().Count().Should().Be(2);

            //act
            PowerShellCli.RemoveItem($"{dir.FullName}\\*");

            //assert
            dir.EnumerateDirectories().Count().Should().Be(0);
            dir.EnumerateFiles().Count().Should().Be(0);
            dir.EnumerateFileSystemInfos().Count().Should().Be(0);
        }
    }
}
