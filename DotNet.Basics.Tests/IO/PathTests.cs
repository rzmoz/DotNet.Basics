using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.IO
{
    [TestFixture]
    public class PathTests
    {
        [Test]
        [TestCase("myFolder\\myFolder\\", "myFolder", true)]//folder with trailing delimiter
        [TestCase("myFolder\\myFolder", "myFolder", true)]//folder without trailing delimiter
        [TestCase("myFolder\\myFile.txt", "myFile.txt", false)]//file with extension
        public void Name_Parsing_NameIsParsed(string fullPath, string expectedName, bool isFolder)
        {
            var path = new Path(fullPath, isFolder);
            //assert
            path.Name.Should().Be(expectedName);
        }

        [Test]
        [TestCase("myFolder\\myFolder\\", "", true)]//folder
        [TestCase("myFolder\\myFolder", "", false)]//folder without extension
        [TestCase("myFile.txt", ".txt", false)]//file with extension
        public void Extensions_Parsing_ExtensionIsParsed(string name, string extension, bool isFolder)
        {
            var path = new Path(name, isFolder);
            //assert
            path.Extension.Should().Be(extension);
        }

        [Test]
        [TestCase("myFolder\\myFolder\\", "myFolder", true)]//folder with trailing delimiter
        [TestCase("myFolder\\myFolder", "myFolder", true)]//folder without trailing delimiter
        [TestCase("myFolder\\myFile", "myFile", false)]//file without extension
        [TestCase("myFolder\\myFile.txt", "myFile.txt", false)]//file with extension
        public void FullName_Parsing_NameIsParsed(string fullPath, string expectedName, bool isFolder)
        {
            var path = new Path(fullPath, isFolder);

            if (isFolder)
                fullPath = fullPath.EnsureSuffix(path.Delimiter.ToChar());

            path.FullName.Should().Be(fullPath);
        }
        [Test]
        [TestCase("myFolder\\myFolder\\", "myFolder", true)]//folder with trailing delimiter
        [TestCase("myFolder\\myFolder", "myFolder", true)]//folder without trailing delimiter
        [TestCase("myFolder\\myFile", "myFile", false)]//file without extension
        [TestCase("myFolder\\myFile.txt", "myFile", false)]//file with extension
        public void NameWithoutExtension_Parsing_NameIsParsed(string fullPath, string expectedName, bool isFolder)
        {
            var path = new Path(fullPath, isFolder);
            //assert
            path.NameWithoutExtension.Should().Be(expectedName);
        }


        [Test]
        [TestCase("\\\\", "myFolder/Myfolder2/", "\\\\myFolder\\Myfolder2\\")]//network folder
        [TestCase("http://", "myFolder/Myfolder2/", "http://myFolder/Myfolder2/")]//uri
        [TestCase(null, "\\\\myFolder\\Myfolder2\\", "\\\\myFolder\\Myfolder2\\")]//network folder
        public void Ctor_Protocol_ProtocolIsPartOfPath(string protocol, string pathSegment, string expectedPath)
        {
            var path = new Path(protocol, pathSegment);
            path.ToString().Should().Be(expectedPath);
            path.ToString().Should().Be(path.FullName);
        }

        [Test]
        [TestCase("myFolder/DetectDelimiter/", PathDelimiter.Slash)]//delimiter detected
        [TestCase("myFolder\\DetectDelimiter\\", PathDelimiter.Backslash)]//delimiter detected
        [TestCase("myFolder/DetectDelimiter\\", PathDelimiter.Slash)]//delimiter detected
        [TestCase("myFolder\\DetectDelimiter//", PathDelimiter.Backslash)]//delimiter detected
        [TestCase("DetectDelimiter", PathDelimiter.Backslash)]//delimiter fallback
        public void Delimiter_Detection_DelimiterDetected(string pathInput, PathDelimiter delimiter)
        {
            var path = new Path(pathInput);
            path.Delimiter.Should().Be(delimiter);
        }

        [Test]
        [TestCase("myFolder/DetectDelimiter/", true)]//delimiter detected
        [TestCase("myFolder\\DetectDelimiter\\", true)]//delimiter detected
        [TestCase("myFolder/DetectDelimiter", false)]//delimiter fallback
        public void IsFolder_Detection_IsFolderIsDetected(string pathInput, bool isFolder)
        {
            var path = new Path(pathInput);
            path.IsFolder.Should().Be(isFolder);
        }

        [Test]
        [TestCase("myFolder/DetectDelimiter/")]
        [TestCase("myFolder\\DetectDelimiter\\")]
        public void ToString_Delimiter_(string pathInput)
        {
            var path = new Path(pathInput);

            var pathWithSlash = path.ToString(PathDelimiter.Slash);
            var pathWithBackSlash = path.ToString(PathDelimiter.Backslash);

            pathWithSlash.Should().Be(pathInput.Replace('\\', '/'), PathDelimiter.Slash.ToName());
            pathWithBackSlash.Should().Be(pathInput.Replace('/', '\\'), PathDelimiter.Backslash.ToName());

        }
    }
}
