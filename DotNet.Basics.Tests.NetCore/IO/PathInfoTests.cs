using System;
using System.IO;
using DotNet.Basics.IO;
using DotNet.Basics.Tests.IO.Testa;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.IO
{
    public class PathInfoTests
    {
        [Theory]
        [InlineData(@"c:\my\path", @"c:\my\path", PathSeparator.Backslash)]//bs to bs
        [InlineData(@"c:\my\path", @"c:/my/path", PathSeparator.Slash)]//bs to bs
        [InlineData(@"c:/my/path", @"c:/my/path", PathSeparator.Slash)]//s to  s
        [InlineData(@"c:/my/path", @"c:\my\path", PathSeparator.Backslash)]//s to bs
        public void RawPath_PathSeparator_SeparatorIsOverridden(string path, string expected, char separator)
        {
            //def path separator
            var pi = path.ToPath(separator);
            pi.RawPath.Should().Be(expected);
        }

        [Theory]
        //files
        [InlineData(@"c:\my\path", @"c:\my\path")]//absolute single path
        [InlineData(@"my\path", @"\my\path")]//relative single path with starting delimiter
        [InlineData(@"my\path", @"my\path")]//relative single path without starting delimiter
        //dirs
        [InlineData(@"c:\my\path\", @"c:\my\path\")]//absolute single path
        [InlineData(@"my\path\", @"\my\path\")]//relative single path with starting delimiter
        [InlineData(@"my\path\", @"my\path\")]//relative single path without starting delimiter
        //segments
        [InlineData(@"c:\my\path\", @"c:\my\", @"\path\")]//absolute segmented path
        [InlineData(@"my\path\", @"\my\", @"\path\")]//relative segmented path with starting delimiter
        [InlineData(@"my\path\", @"my\", @"\path\")]//relative segmented path without starting delimiter
        public void RawPath_RawPathParsing_RawPathIsParsed(string expected, string path, params string[] segments)
        {
            //def path separator
            var pi = path.ToPath(segments);
            pi.RawPath.Should().Be(expected);

            //alt path separator
            var altPi = path.ToPath(PathSeparator.Slash, segments);
            var altExpected = pi.RawPath.Replace(PathSeparator.Backslash, PathSeparator.Slash);

            altPi.RawPath.Should().Be(altExpected);
        }

        [Fact]
        public void Add_Immutable_AddShouldBeImmutable()
        {
            const string path = "root";
            var root = path.ToPath();
            root.Add("sazas");//no change to original path
            root.RawPath.Should().Be(path);
        }

        [Theory]
        [InlineData("SomeDir\\MyFile.txt", "MyFile.txt")]//has extension
        [InlineData("SomeDir\\MyFile", "MyFile")]//no extension
        [InlineData("SomeDir\\.txt", ".txt")]//only extension
        [InlineData(null, "")]//name is null
        public void Name_NameWithExtension_NameIsFound(string name, string expected)
        {
            var file = name.ToFile();
            file.Name.Should().Be(expected, nameof(file.Name));
        }


        [Theory]
        [InlineData("myFolder\\myFolder\\", "myFolder")]//folder with trailing delimiter
        [InlineData("myFolder\\myFolder", "myFolder")]//folder without trailing delimiter
        [InlineData("myFolder\\myFile.txt", "myFile.txt")]//file with extension
        public void Name_Parsing_NameIsParsed(string fullPath, string expectedName)
        {
            var path = fullPath.ToPath();
            //assert
            path.Name.Should().Be(expectedName);
        }

        [Theory]
        [InlineData("myFolder/DetectDelimiter/", PathSeparator.Slash)]//delimiter detected
        [InlineData("myFolder\\DetectDelimiter\\", PathSeparator.Backslash)]//delimiter detected
        [InlineData("myFolder/DetectDelimiter\\", PathSeparator.Slash)]//delimiter detected
        [InlineData("myFolder\\DetectDelimiter//", PathSeparator.Backslash)]//delimiter detected
        [InlineData("DetectDelimiter", PathSeparator.Backslash)]//delimiter fallback
        public void Delimiter_Detection_DelimiterDetected(string pathInput, char delimiter)
        {
            var path = pathInput.ToPath();
            path.Separator.Should().Be(delimiter, pathInput);
        }

        [Theory]
        [InlineData("myFolder/DetectDelimiter/", true)]//delimiter detected
        [InlineData("myFolder\\DetectDelimiter\\", true)]//delimiter detected
        [InlineData("myFolder/DetectDelimiter", false)]//delimiter fallback
        public void IsFolder_Detection_IsFolderIsDetected(string pathInput, bool isFolder)
        {
            var path = pathInput.ToPath();
            path.IsFolder.Should().Be(isFolder);
        }

        [Theory]
        [InlineData("myFolder/DetectDelimiter/", true)]//folder with delimiter in the end
        [InlineData("myFolder/DetectDelimiter", false)]//folder withouth delimiter in the end
        [InlineData("myFolder/myFile.txt", false)]//delimiter fallback
        public void IsFolder_Formatting_FolderExtensionIsOutput(string pathInput, bool isFolder)
        {
            var path = pathInput.ToPath();
            path.IsFolder.Should().Be(isFolder);
            var formatted = path.ToString();
            if (isFolder)
                formatted.Should().EndWith(path.Separator.ToString());
            else
                formatted.Should().NotEndWith(path.Separator.ToString());
        }

        [Theory]
        [InlineData("myFolder/DetectDelimiter/")]
        [InlineData("myFolder\\DetectDelimiter\\")]
        public void ToString_Autodetect_DelimiterIsDetected(string pathInput)
        {
            var path = pathInput.ToPath();

            var pathWithSlash = path.ToString().Replace(PathSeparator.Backslash, PathSeparator.Slash);
            var pathWithBackSlash = path.ToString().Replace(PathSeparator.Slash, PathSeparator.Backslash);

            pathWithSlash.Should().Be(pathInput.Replace('\\', '/'), PathSeparator.Slash.ToString());
            pathWithBackSlash.Should().Be(pathInput.Replace('/', '\\'), PathSeparator.Backslash.ToString());
        }

        [Theory]
        [InlineData(@"c:\myParent\myFolder\", true)]
        [InlineData(@"c:\myParent\myFile", false)]
        public void Directory_GetDir_Dir(string folder, bool isFolder)
        {
            var path = folder.ToPath();

            var dir = path.Directory();

            dir.FullPath().Should().Be(isFolder ? path.FullPath() : path.Parent.FullPath());
        }

        [Fact]
        public void Parent_FullPath_ParentIsResolved()
        {
            var currentDir = TestRoot.Dir;
            var dir = currentDir.Add(@"Parent_NameOnlySourceDir_ParentIsResolved");

            dir.Parent.FullPath().Should().Be(currentDir.FullPath());
        }


        [Theory]
        [InlineData(null, null)]
        [InlineData(@"c:\", null)]
        [InlineData(@"myFolder\", null)]
        [InlineData(@"myParent\myFolder\", @"myParent\")]
        [InlineData(@"myParent\myFile.txt", @"myParent\")]
        [InlineData(@"c:\myParent\myFolder\", @"c:\myParent\")]
        [InlineData(@"c:\myParent\myFile.txt", @"c:\myParent\")]
        public void Parent_DirUp_GetParent(string folder, string expectedParent)
        {
            var path = folder.ToPath();

            var found = path.Parent?.RawPath;
            found.Should().Be(expectedParent, folder);
        }



        [Theory]
        [InlineData("myFolder\\", "dir\\", true)]//backslash all dirs
        [InlineData("myFolder\\", "file.txt", true)]//backslash dir remains when file added
        [InlineData("myfile", "dir//", false)]//slash file remains when dir added - should throw exception?
        [InlineData("myfile.txt", "file.txt", false)]//slash file remains when dir added - should throw exception?
        public void Add_KeepIsFolder_IsFolderIsUnchagedRegardlesOfSegmentsAdded(string root, string newSegment, bool expectedIsFolder)
        {
            var path = root.ToPath();

            //assert before add
            path.IsFolder.Should().Be(expectedIsFolder);

            //act
            path = path.Add(newSegment);

            //assert
            path.IsFolder.Should().Be(expectedIsFolder);
        }

        [Theory]
        [InlineData("myFolder", "")]//empty
        [InlineData("myFolder", null)]//null
        [InlineData("myFolder", "  ")]//spaces
        public void Add_EmptySegments_PathIsUnchanged(string root, string newSegment)
        {
            var path = root.ToPath().Add(newSegment);

            //assert
            path.RawPath.Should().Be(root);
        }

        [Fact]
        public void DeleteIfExists_DirExists_DirIsDeleted()
        {
            var dir = TestRoot.Dir.ToDir("DeleteIfExists_DirExists_DirIsDeleted");
            dir.CreateIfNotExists();
            dir.Exists().Should().BeTrue();

            //act
            dir.DeleteIfExists();

            //assert
            dir.Exists().Should().BeFalse();
        }


        [Fact]
        public void DeleteIfExists_DeleteLongNamedDir_DirIsDeleted()
        {
            //arrange
            //we set up a folder with a really long name
            const string testDirName = "DeleteIfExists_DeleteLongNamedDir_DirIsDeleted";

            var rootTestdir = TestRoot.Dir.ToDir(testDirName);
            rootTestdir.CleanIfExists();
            var identicalSubDir = rootTestdir;
            try
            {
                //we keep adding sub folders until we reach our limit - whichever comes first
                for (var level = 0; level < 15; level++)
                {
                    identicalSubDir.CreateIfNotExists();
                    identicalSubDir = identicalSubDir.ToDir(testDirName);
                }
            }
            catch (System.IO.PathTooLongException)
            {
                identicalSubDir.CleanIfExists();
            }

            rootTestdir.Exists().Should().BeTrue();

            //act
            var deleted = rootTestdir.DeleteIfExists();

            //assert
            deleted.Should().BeTrue();
            rootTestdir.Exists().Should().BeFalse();
        }

        [Fact]
        public void Delete_DeleteFile_FileIsDeleted()
        {
            var testdir = TestRoot.Dir.Add(@"Delete_DeleteFile_FileIsDeleted").ToDir();
            testdir.CleanIfExists();
            var testFile = testdir.ToFile("blaaaah.txt").WriteAllText("Blaaaah!");

            testFile.Exists().Should().BeTrue("File should have been created");

            testFile.DeleteIfExists();

            testFile.Exists().Should().BeFalse("File should have been deleted");
        }


        [Fact]
        public void GetFullPath_CallSystemIo_PathsAreIdentical()
        {
            var relativePath = "GetFullPath_CallSystemIo_PathsAreIdentical";

            var systemDotIoDotPath = System.IO.Path.GetFullPath(relativePath);
            var systemIoPath = relativePath.ToPath().FullPath();

            systemIoPath.Should().Be(systemDotIoDotPath);
        }

        [Fact]
        public void Exists_ThrowIfNotFound_ExceptionIsThrown()
        {
            var path = TestRoot.Dir.ToDir("Exists_ThrowIfNotFound_ExceptionIsThrown");

            path.DeleteIfExists();

            Action action = () => path.Exists(throwIoExceptionIfNotExists: true);

            action.ShouldThrow<IOException>().WithMessage($"{path.FullPath()} not found");
        }

        [Fact]
        public void Exists_PathExists_Works()
        {
            var path = TestRoot.Dir.ToDir("Exists_PathExists_Works");

            path.DeleteIfExists();

            path.Exists().Should().BeFalse();

            path.CreateIfNotExists();

            path.Exists().Should().BeTrue();
        }

        [Fact]
        public void Exists_LongDirPath_NoExceptionIsThrown()
        {
            var path = TestRoot.Dir.ToDir("DirWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\");
            path.Exists().Should().BeFalse();
        }
        [Fact]
        public void Exists_LongFilePath_NoExceptionIsThrown()
        {
            var path = TestRoot.Dir.ToFile("FileWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm");
            path.Exists().Should().BeFalse();
        }

        [Theory]
        //relative dir
        [InlineData(".\\DirWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\")]
        //absolute dir
        [InlineData("c:\\DirWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\")]
        //relative file
        [InlineData(".\\FileWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm")]
        //absolute file
        [InlineData("c:\\FileWithMoreThan260Chars\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm\\mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm")]
        public void FullPath_LongPath_LongPathsAreSupported(string p)
        {
            Action action = () => p.ToPath().FullPath();

            var temp = p.ToPath().FullPath();//no exceptions are thrown

            action.ShouldNotThrow();
        }

        [Fact]
        public void FullPath_Resolve_FullPathIsResolved()
        {
            var inputPath = TestRoot.Dir.ToFile("io", "testa", "TextFile1.txt");
            var expectedPath = new TestFile1().RawPath;
            inputPath.Should().NotBe(expectedPath);

            inputPath.FullPath().ToLowerInvariant().Should().Be(expectedPath.ToLowerInvariant());
        }
    }
}

