using System;
using System.IO;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;
using Path = DotNet.Basics.IO.Path;

namespace DotNet.Basics.Tests.IO
{
    [TestFixture]
    public class PathTests
    {
        [Test]
        //relative dir
        [TestCase(".\\FileWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\")]
        //absolute dir
        [TestCase("c:\\FileWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\")]
        //relative file
        [TestCase(".\\FileWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm")]
        //absolute file
        [TestCase("c:\\FileWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm")]
        public void FullName_LongPath_LongPathsAreSupported(string p)
        {
            var path = new Path(p);

            var expectedP = p;
            if (expectedP.StartsWith("."))
                expectedP = new DirectoryInfo(".").FullName.ToPath(p.RemovePrefix(".\\")).FullName;

            path.FullName.Should().Be(expectedP);//no exceptions
        }

        [Test]
        public void Add_Immutable_AddShouldBeImmutable()
        {
            var root = "root".ToPath();
            root.Add("sazas");
            root.RawName.Should().Be("root");
        }


        [Test]
        public void FullName_SysstemIoCompliance_RelativePathsAreResolvedToSame()
        {
            var relativePath = "myfile.txt";
            var systemIo = new FileInfo(relativePath);
            var path = new Path(relativePath);

            System.IO.Path.GetFullPath(path.FullName).Should().Be(systemIo.FullName);

        }

        [Test]
        [TestCase(@"myFolder\", null)]
        [TestCase(@"myParent\myFolder\", @"myParent\")]
        [TestCase(@"myParent\myFile", @"myParent\")]
        [TestCase(@"c:\myParent\myFolder\", @"c:\myParent\")]
        [TestCase(@"c:\myParent\myFile", @"c:\myParent\")]
        public void Parent_DirUp_GetParent(string folder, string expectedParent)
        {
            var path = new Path(folder);

            var parent = path.Parent;
            try
            {
                parent.RawName.Should().Be(expectedParent);
            }
            catch (NullReferenceException)
            {
                parent.Should().BeNull();
            }
        }

        [Test]
        [TestCase(@"c:\myParent\myFolder\", true)]
        [TestCase(@"c:\myParent\myFile", false)]
        public void Directory_GetDir_Dir(string folder, bool isFolder)
        {
            var path = new Path(folder, isFolder ? DetectOptions.SetToDir : DetectOptions.SetToFile);

            var dir = path.Directory;

            dir.FullName.Should().Be(isFolder ? path.FullName : path.Parent.FullName);
        }

        [Test]
        [TestCase("myFolder", "/")]//slash delimiter
        [TestCase("myFolder", "\\")]//backslash delimiter
        public void Add_AutoDetec_PathIsUpdatedToNewType(string root, string newSegment)
        {
            var path = new Path(root).Add(newSegment);

            //assert
            path.RawName.Should().Be(root + path.Delimiter.ToChar());
        }

        [Test]
        [TestCase("myFolder", "")]//empty
        [TestCase("myFolder", null)]//null
        [TestCase("myFolder", "  ")]//spaces
        public void Add_EmptySegments_PathIsUnchanged(string root, string newSegment)
        {
            var path = new Path(root).Add(newSegment);

            //assert
            path.RawName.Should().Be(root);
        }

        [Test]
        [TestCase("myFolder\\myFolder\\", "myFolder", true)]//folder with trailing delimiter
        [TestCase("myFolder\\myFolder", "myFolder", true)]//folder without trailing delimiter
        [TestCase("myFolder\\myFile.txt", "myFile.txt", false)]//file with extension
        public void Name_Parsing_NameIsParsed(string fullPath, string expectedName, bool isFolder)
        {
            var path = new Path(fullPath, isFolder ? DetectOptions.SetToDir : DetectOptions.SetToFile);
            //assert
            path.Name.Should().Be(expectedName);
        }

        [Test]
        [TestCase("myFolder\\myFolder\\", "", true)]//folder
        [TestCase("myFolder\\myFolder", "", false)]//folder without extension
        [TestCase("myFile.txt", ".txt", false)]//file with extension
        public void Extensions_Parsing_ExtensionIsParsed(string name, string extension, bool isFolder)
        {
            var path = new Path(name, isFolder ? DetectOptions.SetToDir : DetectOptions.SetToFile);
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
            var path = new Path(fullPath, isFolder ? DetectOptions.SetToDir : DetectOptions.SetToFile);

            if (isFolder)
                fullPath = fullPath.EnsureSuffix(path.Delimiter.ToChar());

            path.RawName.Should().Be(fullPath);
        }
        [Test]
        [TestCase("myFolder\\myFolder\\", "myFolder", true)]//folder with trailing delimiter
        [TestCase("myFolder\\myFolder", "myFolder", true)]//folder without trailing delimiter
        [TestCase("myFolder\\myFile", "myFile", false)]//file without extension
        [TestCase("myFolder\\myFile.txt", "myFile", false)]//file with extension
        public void NameWithoutExtension_Parsing_NameIsParsed(string fullPath, string expectedName, bool isFolder)
        {
            var path = new Path(fullPath, isFolder ? DetectOptions.SetToDir : DetectOptions.SetToFile);
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
            path.ToString().Should().Be(path.RawName);
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
        [TestCase("myFolder/DetectDelimiter/", true)]//folder with delimiter in the end
        [TestCase("myFolder/DetectDelimiter", true)]//folder withouth delimiter in the end
        [TestCase("myFolder/myFile.txt", false)]//delimiter fallback
        public void IsFolder_Formatting_FolderExtensionIsOutput(string pathInput, bool isFolder)
        {
            var path = pathInput.ToPath(isFolder ? DetectOptions.SetToDir : DetectOptions.SetToFile);
            path.IsFolder.Should().Be(isFolder);
            var formatted = path.ToString();
            if (isFolder)
                formatted.Should().EndWith(path.Delimiter.ToChar().ToString());
            else
                formatted.Should().NotEndWith(path.Delimiter.ToChar().ToString());
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
