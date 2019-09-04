using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.IO;
using DotNet.Basics.PowerShell;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.PowerShell
{
    public class GetChildItemCmdletTests : TestWithHelpers
    {
        public GetChildItemCmdletTests(ITestOutputHelper output, string testPathPrefix = null) : base(output, testPathPrefix)
        {
        }

        [Fact]
        public void GetChildItem_WithFilter_FilteredItemsAreFound()
        {
            ArrangeActAssertPaths(async dir =>
            {
                dir.ToFile("Testa.myFile1.txt").WriteAllText("nothing1");
                var file1Name = dir.ToFile("Testa.myFile1.json").WriteAllText("nothing1").FullName();
                var file2Name = dir.CreateSubDir("subDir").ToFile("Testa.myFile2.json").WriteAllText("nothing2").FullName();
                dir.ToFile("SomethingElse.myFile1.txt").WriteAllText("nothing3");

                //act
                var result = PowerShellCli.RunCmdlet(new GetChildItemCmdlet(dir.FullName()).WithFilter("*.json").WithRecurse());

                //assert
                var parsedResult = result.Select(path => path.ToString().ToLowerInvariant()).OrderByDescending(p => p).ToList();

                parsedResult.Count.Should().Be(2);
                parsedResult.First().Should().Be(file1Name.ToLowerInvariant());
                parsedResult.Last().Should().Be(file2Name.ToLowerInvariant());
            });
        }
    }
}

