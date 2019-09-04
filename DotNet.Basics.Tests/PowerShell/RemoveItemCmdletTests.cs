using System.Linq;
using DotNet.Basics.IO;
using DotNet.Basics.PowerShell;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.PowerShell
{
    public class RemoveItemCmdletTests : TestWithHelpers
    {
        public RemoveItemCmdletTests(ITestOutputHelper output, string testPathPrefix = null) : base(output, testPathPrefix)
        {
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
                PowerShellCli.RunCmdlet(new RemoveItemCmdlet($@"{dir.FullName()}\*"));

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
                PowerShellCli.RunCmdlet(new RemoveItemCmdlet($@"{dir.FullName()}/Testa.*.txt").WithForce().WithRecurse());

                //assert
                dir.EnumerateFiles().Count().Should().Be(2);
            });
        }
    }
}
