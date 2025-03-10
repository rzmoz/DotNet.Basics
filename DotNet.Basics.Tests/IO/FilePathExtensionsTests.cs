﻿using System;
using System.IO;
using System.Threading.Tasks;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using DotNet.Basics.Tests.IO.Testa;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.IO
{
    public class FilePathExtensionsTests(ITestOutputHelper output) : TestWithHelpers(output)
    {
        [Fact]
        public async Task CreateRead_Streams_ContentIsWrittenToDiskAndReadBackAgain()
        {
            await WithTestRootAsync(async dir =>
            {
                var expectedText =
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.\r\nDuis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\r\n";

                var testFile = dir.ToFile(Guid.NewGuid().ToString());
                await using var textStream = expectedText.ToStream();
                {
                    await using var
                        fileStream =
                            testFile.Create(
                                textStream); //ensure this code is executed in a separate block from read to ensure proper disposal of write operation
                }

                var observedText = await testFile.OpenRead().ReadToEndAsync();
                observedText.Should().Be(expectedText);
            });
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ToFile_PathIsNull_PathIsEmpty(string path)
        {
            var actual = path.ToFile();

            actual.RawPath.Should().BeEmpty();
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
                targetFile.ReadAllText().Should().Be("blaaah!"); //assert target file exists

                TestFile1 testFile1 = null;
                WithTestRoot(testRoot => testFile1 = new TestFile1(testRoot));
                testFile1.CopyTo(testDir, targetFile.Name, true); //ensure file exists in target

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
        public void ReadAllBytes_ReadBytes_BytesAreRead()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var testFile = new TestFile1(testDir);
                testFile.Exists().Should().BeFalse();
                testFile.WriteAllText("Hello World!");
                var bytes = testFile.ReadAllBytes();
                var content = System.Text.Encoding.UTF8.GetString(bytes);
                content.Should().EndWithEquivalentOf("Hello World!");
            });
        }

        [Fact]
        public void ReadAllTextThrowIfNotExists_SilenceWhenDirNotFound_EmptyReturned()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var file = testDir.ToFile("NotFOund.asd");
                var content = file.ReadAllText(IfNotExists.Mute);
                content.Should().BeEmpty();
            });
        }

        [Fact]
        public void ReadAllTextAsync_FileFound_ContentIsReturned()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var file = testDir.ToFile("NotFOund.asd");
                var content = Lorem.Ipsum(25);
                file.WriteAllText(content);

                var contentFromDisk = file.ReadAllTextAsync().Result;

                contentFromDisk.Should().Be(content);
            });
        }

        [Fact]
        public void ReadAllTextThrowIfNotExists_SilenceWhenFileNotFound_EmptyIsReturned()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var file = testDir.ToFile(@"ReadAllTextThrowIfNotExists_SilenceWhenFileNotFound_NullIsReturned", "NotFOund.asd");
                file.Directory.CreateIfNotExists();
                var content = file.ReadAllText(IfNotExists.Mute);
                content.Should().BeEmpty();
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
                var file = testDir.ToFile("NotFOundxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.txt");
                file.Directory.CreateIfNotExists();
                Action action = () => file.ReadAllText();
                action.Should().Throw<FileNotFoundException>();
            });
        }

        [Fact]
        public void OpenWrite_MultiWrite_FileIsWrittenToMultipleTimes()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var file = testDir.ToFile("write-me.txt");

                using var writer1 = file.OpenWrite();
                writer1.WriteLine("line 1");
                writer1.Flush();

                using var reader1 = file.OpenRead();
                reader1.ReadLine().Should().Be("line 1");

                using var writer2 = file.OpenWrite();
                writer2.WriteLine("line 2");
                writer2.Flush();

                using var reader2 = file.OpenRead();
                reader2.ReadLine().Should().Be("line 2");
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
                targetFile.Directory.CreateIfNotExists();
                //ensure file exists
                File.WriteAllText(targetFile.FullName, @"mycontent");
                targetFile.Exists().Should().BeTrue();

                //act
                Action action = () => targetFile.WriteAllText(@"random", overwrite: false);

                action.Should().Throw<IOException>()
                    .WithMessage(
                        $"Target file already exists. Set overwrite to true to ignore existing file: {targetFile.FullName}");
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
                targetFile.Directory.CreateIfNotExists();
                File.WriteAllText(targetFile.FullName, initialContent);
                targetFile.Exists().Should().BeTrue();
                File.ReadAllText(targetFile.FullName).Should().Be(initialContent);

                //act
                targetFile.WriteAllText(updatedContent, overwrite: true);

                File.ReadAllText(targetFile.FullName).Should().Be(updatedContent);
            });
        }

        [Fact]
        public void ToTargetFile_MultipleDirCombine_TargetFileHasNewDir()
        {
            const string fileName = "myFile.temp";
            var sourceFile = fileName.ToFile();

            var targetDir = @"\MyPath".ToDir("subfolder1", "subfolder2");
            var targetfile = targetDir.ToFile(sourceFile.Name);

            targetfile.FullName.Should().EndWith(@"/MyPath/subfolder1/subfolder2/" + fileName);
        }

        [Fact]
        public void ToTargetFile_SingleDirCombine_TargetFileHasNewDir()
        {
            const string fileName = @"/Something\myFile.temp";
            var sourceFile = fileName.ToFile();

            var targetfile = @"\MyPath".ToFile(sourceFile.Name);

            targetfile.FullName.Should().EndWith(@"/MyPath/myFile.temp");
        }
    }
}