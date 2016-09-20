using DotNet.Basics.IO;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.IO
{
    [TestFixture]
    public class FileSystemInfoExtensionTests
    {
        [Test]
        [TestCase(@"io\testsources\", "CopyTo_IsPathTypeAgnostic_ItemIsCopied_NewFolder", @"CopyTo_IsPathTypeAgnostic_ItemIsCopied_NewFolder", true, true)]
        [TestCase(@"io\testsources\textfile1.txt", "CopyTo_IsPathTypeAgnostic_ItemIsCopied_NewFolder", @"CopyTo_IsPathTypeAgnostic_ItemIsCopied_NewFolder\textfile1.txt", false, true)]
        [TestCase(@"io\testsources\textfile1.txt", @"CopyTo_IsPathTypeAgnostic_ItemIsCopied_NewFolder\newfile.txt", @"CopyTo_IsPathTypeAgnostic_ItemIsCopied_NewFolder\newfile.txt", false, false)]
        public void CopyTo_IsPathTypeAgnostic_ItemIsCopied(string source, string destination, string expectedFinalPath, bool sourceIsFolder, bool destinationIsFolder)
        {
            var sourceFsi = TestContext.CurrentContext.TestDirectory.ToPath(sourceIsFolder, source);
            var destinationFsi = TestContext.CurrentContext.TestDirectory.ToPath(destinationIsFolder, destination);
            var expectedFinalPathFsi = TestContext.CurrentContext.TestDirectory.ToPath(sourceIsFolder, expectedFinalPath);
            destinationFsi.DeleteIfExists();
            if (destinationFsi.IsFolder)
                destinationFsi.ToDir().CleanIfExists();

            destinationFsi.Exists().Should().BeFalse("Desitnation Fsi");
            expectedFinalPathFsi.DeleteIfExists();
            expectedFinalPathFsi.Exists().Should().BeFalse("Expected Final Path Fsi");

            //act
            sourceFsi.CopyTo(destinationFsi);

            //assert
            expectedFinalPathFsi.Exists().Should().BeTrue("Expected Final Path Fsi");

            //ensure content is copied
            if (sourceFsi.IsFolder && destinationFsi.IsFolder)
            {
                foreach (var content in sourceFsi.ToDir().EnumeratePaths())
                {
                    PathInfo expectedDestinationContentFsi;
                    if (content.IsFolder)
                        expectedDestinationContentFsi = destinationFsi.ToDir(content.Name);
                    else
                        expectedDestinationContentFsi = destinationFsi.ToFile(content.Name);
                    expectedDestinationContentFsi.Exists().Should().BeTrue(content.FullName);
                }
            }
        }
    }
}
