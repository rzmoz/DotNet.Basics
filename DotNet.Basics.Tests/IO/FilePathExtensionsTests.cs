using System;
using System.IO;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using DotNet.Basics.Tests.IO.Testa;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.IO
{
    public class FilePathExtensionsTests : TestWithHelpers
    {
        public FilePathExtensionsTests(ITestOutputHelper output) : base(output)
        { }

        [Fact]
        public void ToFile_PathIsNull_NullIsReturned()
        {
            string nullPath = null;

            var actual = nullPath.ToFile();

            actual.Should().BeNull();
        }
        [Fact]
        public void ToFile_PathIsEmpty_NullIsReturned()
        {
            string emptyPath = string.Empty;

            var actual = emptyPath.ToFile();

            actual.Should().BeNull();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CopyTo_EnsureTargetDir_TargetDirIsEnsured(bool ensureTargetDir)
        {
            ArrangeActAssertPaths(dir =>
            {
                var testDir = dir.Add($"{ensureTargetDir}");
                testDir.DeleteIfExists();

                TestFile1 testFile1 = null;
                WithTestRoot(testRoot => testFile1 = new TestFile1(testRoot));

                Action action = () => testFile1.CopyTo(testDir, false, ensureTargetDir);

                if (ensureTargetDir)
                {
                    action.Should().NotThrow();
                    testDir.ToFile(testFile1.Name).Exists().Should().BeTrue();
                }
                else
                {
                    action.Should().Throw<IOException>();
                    testDir.ToFile(testFile1.Name).Exists().Should().BeFalse();
                }
            });

        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CopyTo_OverWrite_OverwriteIsObeyed(bool overwrite)
        {
            ArrangeActAssertPaths(dir =>
            {
                var testDir = dir.Add($"{overwrite}");
                var targetFile = testDir.ToFile("myFile.txt");
                targetFile.WriteAllText("blaaah!");
                targetFile.ReadAllText().Should().Be("blaaah!");//assert target file exists

                TestFile1 testFile1 = null;
                WithTestRoot(testRoot => testFile1 = new TestFile1(testRoot));
                testFile1.CopyTo(testDir, targetFile.Name, true);//ensure file exists in target

                Action action = () => testFile1.CopyTo(targetFile, overwrite);

                if (overwrite)
                {
                    action.Should().NotThrow();
                    targetFile.ReadAllText().Should().Be("Hello World!");
                }
                else
                {
                    action.Should().Throw<IOException>();
                }
            });
        }

        [Fact]
        public void MoveTo_RenameFileInSameFolder_FileIsRenamed()
        {
            ArrangeActAssertPaths(testDir =>
            {
                testDir.CleanIfExists();
                var sourceFile = testDir.ToFile("blaaOld.txt");
                var tagetFile = testDir.ToFile("blaaNew.txt");
                sourceFile.WriteAllText("blaa");

                sourceFile.Exists().Should().BeTrue("Source file before move");
                tagetFile.Exists().Should().BeFalse("Target file before move");

                sourceFile.MoveTo(tagetFile);

                sourceFile.Exists().Should().BeFalse("Source file after move");
                tagetFile.Exists().Should().BeTrue("Target file after move");
            });

        }

        [Fact]
        public void ReadAllBytes_ReadBytes_BytesAreREad()
        {
            ArrangeActAssertPaths(testDir =>
            {
                TestFile1 testFile = new TestFile1(testDir);

                var bytes = testFile.ReadAllBytes();
                var content = System.Text.Encoding.UTF8.GetString(bytes );
                content.Should().EndWithEquivalentOf("Hello World!");
            });
        }
        [Fact]
        public void ReadAllTextThrowIfNotExists_SilenceWhenDirNotFound_NullIsReturned()
        {
            ArrangeActAssertPaths(testDir =>
                {
                    var file = testDir.ToFile("NotFOund.asd");
                    var content = file.ReadAllText(IfNotExists.Mute);
                    content.Should().BeNull();
                });
        }
        [Fact]
        public void ReadAllTextThrowIfNotExists_SilenceWhenFileNotFound_NullIsReturned()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var file = testDir.ToFile(@"ReadAllTextThrowIfNotExists_SilenceWhenFileNotFound_NullIsReturned", "NotFOund.asd");
                file.Directory().CreateIfNotExists();
                var content = file.ReadAllText(IfNotExists.Mute);
                content.Should().BeNull();
            });
        }

        [Fact]
        public void ReadAllTextThrowIfNotExists_ThrowWhenDirNotFound_ExceptionIsThrown()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var file = testDir.ToFile("NOtFoundDir", "NotFOund.asd");
                Action action = () => file.ReadAllText();
                action.Should().Throw<DirectoryNotFoundException>();
            });

        }
        [Fact]
        public void ReadAllTextThrowIfNotExists_ThrowWhenFileNotFound_ExceptionIsThrown()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var file = testDir.ToFile("NotFOundxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.asd");
                file.Directory().CreateIfNotExists();
                Action action = () => file.ReadAllText();
                action.Should().Throw<FileNotFoundException>();
            });
        }

        [Fact]
        public void ReadAllText_ReadTextFromFile_ContentIsRead()
        {
            ArrangeActAssertPaths(testDir =>
            {
                testDir.CleanIfExists();
                var testFile = testDir.ToFile("blaaaah.txt");

                string testContent = "Blaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaah";

                testFile.WriteAllText(testContent);

                var read = testFile.ReadAllText();

                read.Should().Be(testContent);
            });
        }

        [Fact]
        public void WriteAllText_WhenOverwriteIsFalseAndTargetExists_ExceptionIsThrown()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var targetFile = testDir.ToFile("target.txt");
                targetFile.Directory().CreateIfNotExists();
                //ensure file exists
                File.WriteAllText(targetFile.FullName(), @"mycontent");
                targetFile.Exists().Should().BeTrue();

                //act
                Action action = () => targetFile.WriteAllText(@"random", overwrite: false);

                action.Should().Throw<IOException>().WithMessage($"Target file already exists. Set overwrite to true to ignore existing file: {targetFile.FullName()}");
            });
        }

        [Fact]
        public void WriteAllText_WhenOverwriteIsTrueAndTargetExists_ContentIsReplaced()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var initialContent = "WriteAllText_WhenOverwriteIsTrueAndTargetExists_ContentIsReplaced";
                var updatedContent = "UpdatedContent";


                var targetFile = testDir.ToFile("target.txt");
                targetFile.Directory().CreateIfNotExists();
                File.WriteAllText(targetFile.FullName(), initialContent);
                targetFile.Exists().Should().BeTrue();
                File.ReadAllText(targetFile.FullName()).Should().Be(initialContent);

                //act
                targetFile.WriteAllText(updatedContent, overwrite: true);

                File.ReadAllText(targetFile.FullName()).Should().Be(updatedContent);
            });
        }

        [Fact]
        public void ToTargetFile_MultipleDirCombine_TargetFileHasNewDir()
        {
            const string fileName = "myFile.temp";
            var sourceFile = fileName.ToFile();

            var targetDir = @"c:\MyPath".ToDir("subfolder1", "subfolder2");
            var targetfile = targetDir.ToFile(sourceFile.Name);

            targetfile.FullName().Should().Be(@"c:\MyPath\subfolder1\subfolder2\" + fileName);
        }
        [Fact]
        public void ToTargetFile_SingleDirCombine_TargetFileHasNewDir()
        {
            const string fileName = @"c:\Something\myFile.temp";
            var sourceFile = fileName.ToFile();

            var targetfile = @"c:\MyPath".ToFile(sourceFile.Name);

            targetfile.FullName().Should().Be(@"c:\MyPath\myFile.temp");
        }
    }
}
