using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            FileSystemInfo sourceFsi = Create(source, sourceIsFolder);
            FileSystemInfo destinationFsi = Create(destination, destinationIsFolder);
            FileSystemInfo expectedFinalPathFsi = Create(expectedFinalPath, sourceIsFolder);
            destinationFsi.DeleteIfExists();
            if (destinationIsFolder)
                (destinationFsi as DirectoryInfo).CleanIfExists();

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
                foreach (var content in (sourceFsi as DirectoryInfo).EnumerateFileSystemInfos())
                {
                    FileSystemInfo expectedDestinationContentFsi;
                    if (content is DirectoryInfo)
                        expectedDestinationContentFsi = (destinationFsi as DirectoryInfo).ToDir(content.Name);
                    else
                        expectedDestinationContentFsi = (destinationFsi as DirectoryInfo).ToFile(content.Name);
                    expectedDestinationContentFsi.Exists().Should().BeTrue(content.FullName);
                }
            }
        }

        private FileSystemInfo Create(string path, bool isFolder)
        {
            if (isFolder)
                return new DirectoryInfo(path);
            return new FileInfo(path);
        }
    }
}
