using DotNet.Basics.IO;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.IO
{
    [TestFixture]
    public class FileSystemInfoExtensionTests
    {
        [Test]
        [TestCase(@".\io\testsources\", "CopyTo_IsDerivedTypeAgnostic_ItemIsCopiedNewFolder", @".\CopyTo_IsDerivedTypeAgnostic_ItemIsCopiedNewFolder", true, true)]
        [TestCase(@".\io\testsources\textfile1.txt", "CopyTo_IsDerivedTypeAgnostic_ItemIsCopied", @".\CopyTo_IsDerivedTypeAgnostic_ItemIsCopied\textfile1.txt", false, true)]
        [TestCase(@".\io\testsources\textfile1.txt", @"CopyTo_IsDerivedTypeAgnostic_ItemIsCopied\newfile.txt", @".\CopyTo_IsDerivedTypeAgnostic_ItemIsCopied\newfile.txt", false, false)]
        public void CopyTo_IsDerivedTypeAgnostic_ItemIsCopied(string source, string destination, string expectedFinalPath, bool sourceIsFolder, bool destinationIsFolder)
        {
            var sourceFsi = source.ToPath(sourceIsFolder);
            var destinationFsi = destination.ToPath(destinationIsFolder);
            var expectedFinalPathFsi = expectedFinalPath.ToPath(sourceIsFolder);
            destinationFsi.DeleteIfExists();
            if (destinationIsFolder)
                (destinationFsi as DirPath).CleanIfExists();

            destinationFsi.Exists().Should().BeFalse("Desitnation Fsi");
            expectedFinalPathFsi.DeleteIfExists();
            expectedFinalPathFsi.Exists().Should().BeFalse("Expected Final Path Fsi");

            //act
            sourceFsi.CopyTo(destinationFsi);

            //assert
            expectedFinalPathFsi.Exists().Should().BeTrue("Expected Final Path Fsi");

            //ensure content is copied
            if (sourceIsFolder && destinationIsFolder)
            {
                foreach (var content in (sourceFsi as DirPath).EnumeratePaths())
                {
                    Path expectedDestinationContentFsi;
                    if (content is DirPath)
                        expectedDestinationContentFsi = (destinationFsi as DirPath).ToDir(content.Name);
                    else
                        expectedDestinationContentFsi = (destinationFsi as DirPath).ToFile(content.Name);
                    expectedDestinationContentFsi.Exists().Should().BeTrue(content.FullName);
                }
            }
        }
    }
}
