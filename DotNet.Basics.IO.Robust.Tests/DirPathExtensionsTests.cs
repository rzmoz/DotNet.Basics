using System;
using System.Linq;
using DotNet.Basics.Sys;
using DotNet.Basics.TestsRoot;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.IO.Robust.Tests
{
    public class DirPathExtensionsTests: TestWithHelpers
    {
        public DirPathExtensionsTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void ToDir_CombineToDir_FullNameIsCorrect()
        {
            var _testDirRoot = @"K:\testDir";
            var _testDoubleDir = @"\testa\testb";

            var actual = _testDirRoot.ToDir(_testDoubleDir);
            var expected = _testDirRoot + _testDoubleDir + actual.Separator;
            actual.FullName().Should().Be(expected);

            actual = @"c:\BuildLibrary\Folder\Module 2.0.1".ToDir("Website");
            expected = @"c:\BuildLibrary\Folder\Module 2.0.1\Website\";
            actual.FullName().Should().Be(expected);
        }

        [Fact]
        public void CreateIfExists_CreateOptions_ExistingDirIsCleaned()
        {
            //arrange
            var testDir = TestRoot.ToDir("CreateIfExists_CreateOptions_ExistingDirIsCleaned");
            testDir.ToFile("myFile.txt").WriteAllText(@"bllll");

            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Count.Should().Be(1);

            //act
            testDir.CreateIfNotExists();
            testDir.CleanIfExists();

            //assert
            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Count.Should().Be(0);
        }

        [Fact]
        public void CreateIfExists_CreateOptions_ExistingDirIsNotCleaned()
        {
            //arrange
            var testDir = TestRoot.ToDir("CreateIfExists_CreateOptions_ExistingDirIsNotCleaned");
            testDir.DeleteIfExists();
            testDir.ToFile("myFile.txt").WriteAllText(@"bllll");

            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Count.Should().Be(1);

            //act
            testDir.CreateIfNotExists();

            //assert
            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Count.Should().Be(1);
        }
        [Fact]
        public void CleanIfExists_DirDoesntExists_NoActionAndNoExceptions()
        {
            //arrange
            var testDir = TestRoot.ToDir("SOMETHINGTHAT DOESNT EXIST_BLAAAAAA");

            //act
            testDir.CleanIfExists();

            //assert
            testDir.Exists().Should().BeFalse();
        }
        [Fact]
        public void CleanIfExists_RemoveAllContentFromADir_DirIsCleaned()
        {
            //arrange
            var testDir = TestRoot.ToDir("CleanIfExists_RemoveAllContentFromADir_DirIsCleaned");
            testDir.CreateIfNotExists();

            const int numOfTestFiles = 3;

            AddTestContent(testDir, 0, numOfTestFiles);

            testDir.GetFiles().Count.Should().Be(numOfTestFiles);

            //act
            testDir.CleanIfExists();

            testDir.GetFiles().Count.Should().Be(0);
        }

        [Fact]
        public void CopyTo_IncludeSubDirectories_DirIsCopied()
        {
            const int dirDepth = 3;
            var root = TestRoot.ToDir("CopyTo_InclSubDirs_DirIsCop");
            root.DeleteIfExists();
            var currentDir = CreateIdenticalSubdirs(root, dirDepth);
            currentDir.ToFile("myFile.txt").WriteAllText("blaaaa");

            var targetRoot = TestRoot.ToDir(root.Name + "Target");
            var targetDir = targetRoot.CreateSubDir(root.Name);
            targetDir.DeleteIfExists();
            targetDir.Exists().Should().BeFalse();

            root.CopyTo(targetDir, includeSubfolders: true);

            targetDir.Exists().Should().BeTrue();
            GetHierarchyDepth(targetDir).Should().Be(dirDepth + 1);
        }

        private int GetHierarchyDepth(DirPath root)
        {
            var subDir = root.GetDirectories().SingleOrDefault(dir => dir.Name.Equals(root.Name, StringComparison.InvariantCultureIgnoreCase));
            if (subDir == null)
                return 1;
            var subIoDir = subDir.ToDir();
            return GetHierarchyDepth(subIoDir) + 1;
        }

        private void AddTestContent(DirPath dir, int numOfTestDirs, int numOfTestFiles)
        {
            for (var i = 0; i < numOfTestDirs; i++)
            {
                var testFolder = dir.ToDir("MyTestFolder" + i);
                testFolder.CreateIfNotExists();
                testFolder.ToFile($"blaa{i}.txt").WriteAllText("blaaa");
            }
            for (var i = 0; i < numOfTestFiles; i++)
                dir.ToFile($"myFile{i}.txt").WriteAllText("blaaaaa");
        }
        private DirPath CreateIdenticalSubdirs(DirPath root, int maxDepth)
        {
            try
            {
                //we keep adding sub folders until we reach our limit or max levels - whichever comes first
                for (var level = 0; level < maxDepth; level++)
                {
                    var dirName = level % 2 == 0 ? root.Name.ToLower() : root.Name.ToUpper();
                    root = root.CreateSubDir(dirName);
                }
            }
            catch (System.IO.PathTooLongException)
            {
                //we set it to one level up so were sure we can have test content
                root = root.Parent;
                root.CleanIfExists();
            }
            return root;
        }
    }
}
