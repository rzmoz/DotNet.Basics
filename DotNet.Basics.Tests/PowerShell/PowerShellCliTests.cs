using System.Linq;
using DotNet.Basics.PowerShell;

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

        [Fact]
        public void RemoveItem_Filters_OnlyFilteredItemsAreDeleted()
        {
            ArrangeActAssertPaths(dir =>
            {
                dir.ToFile("Testa.myFile1.txt").WriteAllText("nothing1");
                dir.ToFile("Testa.myFile2.txt").WriteAllText("nothing2");
                dir.ToFile("Testa.myFile1.json").WriteAllText("nothing1");
                dir.ToFile("SomethingElse.myFile1.txt").WriteAllText("nothing3");

                dir.EnumerateFiles().Count().Should().Be(4);

                //act
                PowerShellCli.RemoveItem($@"{dir.FullName()}/Testa.*.txt", force: true, recurse: true);

                //assert
                dir.EnumerateFiles().Count().Should().Be(2);
            });
        }
    }
}
