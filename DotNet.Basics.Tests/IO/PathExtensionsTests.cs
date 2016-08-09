using System;
using DotNet.Basics.IO;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.IO
{
    [TestFixture]
    public class PathExtensionsTests
    {
        [Test]
        [TestCase("FOLDER_THAT_DOES_NOT_EXIST_WO_FOLDER_MARKER", false)]//folder that doesnt exist without marker
        [TestCase("FOLDER_THAT_DOES_NOT_EXIST_WITH_FOLDER_MARKER//", true)]//folder that doesnt exist with marker
        [TestCase("IsFolder_DetectIfFolder_FoldersAreDetected", true)]//folder that exists without marker
        [TestCase("IsFolder_DetectIfFolder_FoldersAreDetected\\myfile.txt\\", false)]//file that exists with folder marker
        public void IsFolder_DetectIfFolder_FoldersAreDetected(string input, bool expectedIsFolder)
        {
            var dir = TestContext.CurrentContext.TestDirectory.ToDir("IsFolder_DetectIfFolder_FoldersAreDetected");
            dir.CreateIfNotExists();
            "dummycontent".WriteAllText(dir.ToFile("myfile.txt"));

            var path = TestContext.CurrentContext.TestDirectory.ToPath(input);

            path.IsFolder.Should().Be(expectedIsFolder);
        }

        [Test]
        [TestCase("Exists_TestPath_PathIsVerified", true)]//dir
        [TestCase("Exists_TestPath_PathIsVerified\\file.txt", false)]//file
        public void Exists_TestPath_PathIsVerified(string path, bool isFolder)
        {
            Path p;
            if (isFolder)
                p = TestContext.CurrentContext.TestDirectory.ToDir(path);
            else
                p = TestContext.CurrentContext.TestDirectory.ToFile(path);

            p.DeleteIfExists();
            p.Exists().Should().BeFalse(p.FullName);

            if (p.IsFolder)
                p.ToDir().CreateIfNotExists();
            else
                "dummycontent".WriteAllText(p);

            p.Exists().Should().BeTrue(p.FullName);
        }

        [Test]
        [TestCase("http://",true)]
        [TestCase("http:/", false)]
        [TestCase("http://folder", false)]//
        public void IsProtocol_ProtocolIsDetected(string path, bool expected)
        {
            path.IsProtocol().Should().Be(expected);
        }
        [Test]
        [TestCase("http://", true)]
        [TestCase("http:/", false)]
        [TestCase("http://folder", true)]//
        public void HasProtocol_ProtocolIsDetected(string path, bool expected)
        {
            path.HasProtocol().Should().Be(expected);
        }

        [Test]
        public void SplitSplitSegments_UriSUpport_UriIsSPlitProperly()
        {
            var segments = new[]
            {
                "http://localhost:80/",
                "myfile.aspx"
            };

            var split = segments.SplitToSegments();

            split.Length.Should().Be(3);
            split[0].Should().Be("http://");
            split[1].Should().Be("localhost:80");
            split[2].Should().Be("myfile.aspx");
        }

        [Test]
        [TestCase(PathDelimiter.Slash, '/')]
        [TestCase(PathDelimiter.Backslash, '\\')]
        public void ToChar_DelimiterToChar_DelimiterIsConverted(PathDelimiter delimiter, char expectedChar)
        {
            var foundChar = delimiter.ToChar();
            foundChar.Should().Be(expectedChar);
        }

        [Test]
        [TestCase('/', PathDelimiter.Slash)]
        [TestCase('\\', PathDelimiter.Backslash)]
        public void ToPathDelimiter_CharToDelimiter_CharIsConverted(char delimiter, PathDelimiter expectedPathDelimiter)
        {
            var foundPathDelimiter = delimiter.ToPathDelimiter();
            foundPathDelimiter.Should().Be(expectedPathDelimiter);
        }

        [Test]
        [TestCase('a')]
        [TestCase('1')]
        [TestCase('¤')]
        public void ToPathDelimiter_UnsupportedChar_ExceptionIsThrown(char delimiter)
        {
            Action action = () => delimiter.ToPathDelimiter();
            action.ShouldThrow<NotSupportedException>();
        }

        [Test]
        [TestCase("//pt101", "pt2", PathDelimiter.Slash)]//file
        [TestCase("\\pt101", "pt2", PathDelimiter.Backslash)]//file
        [TestCase("//pt101", "pt2/", PathDelimiter.Slash)]//dir
        [TestCase("\\pt101", "pt2\\", PathDelimiter.Backslash)]//dir
        public void ToPath_Combine_PathIsGenerated(string pt1, string pt2, PathDelimiter pathDelimiter)
        {
            var path = pt1.ToPath(pt2);

            var refPath = pt1 + pathDelimiter.ToChar() + pt2;
            if (path.IsFolder == false)
                refPath = refPath.TrimEnd(pathDelimiter.ToChar());
            refPath = refPath.TrimStart(pathDelimiter.ToChar());
            path.RawName.Should().Be(refPath);
        }


        [Test]
        [TestCase("http://localhost/", "myFolder/myFile.html")]//url
        public void ToPath_Uris_PathIsGenerated(string pt1, string pt2)
        {
            var path = pt1.ToPath(pt2);
            path.FullName.Should().Be("http://localhost/myFolder/myFile.html");
        }


        [Test]
        [TestCase("mypath", false)]//file
        [TestCase("mypath", true)]//file
        public void IsFolder_Set_IsFolderIsSet(string pth, bool isFolder)
        {
            Path p;
            if (isFolder)
                p = pth.ToDir();
            else
                p = pth.ToFile();

            p.IsFolder.Should().Be(isFolder);
        }
    }
}
