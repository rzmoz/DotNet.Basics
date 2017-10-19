using System;
using System.Linq;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.IO
{
    public class DirPathTests
    {
        private const string _path = "c:/mypath";
        private const string _segment = "segment";

        private const string _testDirRoot = @"K:\testDir";
        private const string _testDoubleDir = @"\testa\testb";
        private const string _testSingleDir = @"\testk\";

        [Fact]
        public void Add_Dir_SameTypeIsReturned()
        {
            var dir = _path.ToDir().Add(_segment);

            dir.Should().BeOfType<DirPath>();
            dir.RawPath.Should().Be(_path + $"/{_segment }/");
        }
        [Fact]
        public void ToDir_Create_DirIsCreated()
        {
            var dir = _path.ToDir().ToFile().ToDir(_segment);//different extension methods

            dir.Should().BeOfType<DirPath>();
            dir.RawPath.Should().Be(_path + $"/{_segment }/");
        }

        [Fact]
        public void ToDir_CombineToDir_FullNameIsCorrect()
        {
            var actual = _testDirRoot.ToDir(_testDoubleDir);
            var expected = _testDirRoot + _testDoubleDir + actual.Separator;
            actual.FullPath().Should().Be(expected);

            actual = @"c:\BuildLibrary\Folder\Module 2.0.1".ToDir("Website");
            expected = @"c:\BuildLibrary\Folder\Module 2.0.1\Website\";
            actual.FullPath().Should().Be(expected);
        }

        [Fact]
        public void ToDir_ParentFromCombine_ParentFolderOfNewIsSameAsOrgRoot()
        {
            var rootDir = _testDirRoot.ToDir();
            var dir = rootDir.ToDir(_testSingleDir);
            dir.Parent.Name.Should().Be(rootDir.Name);
        }
        [Fact]
        public void CreateIfExists_CreateOptions_ExistingDirIsCleaned()
        {
            //arrange
            var testDir = TestRoot.Dir.Add("CreateIfExists_CreateOptions_ExistingDirIsCleaned");
            testDir.ToFile("myFile.txt").WriteAllText(@"bllll");

            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Length.Should().Be(1);

            //act
            testDir.CreateIfNotExists();
            testDir.CleanIfExists();

            //assert
            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Length.Should().Be(0);
        }

        [Fact]
        public void CreateIfExists_CreateOptions_ExistingDirIsNotCleaned()
        {
            //arrange
            var testDir = TestRoot.Dir.Add("CreateIfExists_CreateOptions_ExistingDirIsNotCleaned");
            testDir.DeleteIfExists();
            testDir.ToFile("myFile.txt").WriteAllText(@"bllll");

            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Length.Should().Be(1);

            //act
            testDir.CreateIfNotExists();

            //assert
            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Length.Should().Be(1);
        }
        [Fact]
        public void CleanIfExists_DirDoesntExists_NoActionAndNoExceptions()
        {
            //arrange
            var testDir = TestRoot.Dir.Add("SOMETHINGTHAT DOESNT EXIST_BLAAAAAA");

            //act
            testDir.CleanIfExists();

            //assert
            testDir.Exists().Should().BeFalse();
        }
        [Fact]
        public void CleanIfExists_RemoveAllContentFromADir_DirIsCleaned()
        {
            //arrange
            var testDir = TestRoot.Dir.Add("CleanIfExists_RemoveAllContentFromADir_DirIsCleaned");
            testDir.CreateIfNotExists();

            const int numOfTestFiles = 3;

            AddTestContent(testDir, 0, numOfTestFiles);

            testDir.GetFiles().Length.Should().Be(numOfTestFiles);

            //act
            testDir.CleanIfExists();

            testDir.GetFiles().Length.Should().Be(0);
        }

        [Fact]
        public void CopyTo_IncludeSubDirectories_DirIsCopied()
        {
            const int dirDepth = 3;
            var root = TestRoot.Dir.Add("CopyTo_InclSubDirs_DirIsCop");
            root.DeleteIfExists();
            var currentDir = CreateIdenticalSubdirs(root, dirDepth);
            currentDir.ToFile("myFile.txt").WriteAllText("blaaaa");

            var targetRoot = TestRoot.Dir.Add(root.Name + "Target");
            var targetDir = targetRoot.CreateSubDir(root.Name);
            targetDir.DeleteIfExists();
            targetDir.Exists().Should().BeFalse();

            root.CopyTo(targetDir, includeSubfolders: true);

            targetDir.Exists().Should().BeTrue();
            GetHierarchyDepth(targetDir).Should().Be(dirDepth + 1);
        }


        [Fact]
        public void ConsolidateIdenticalSubfolders_LookDepthLimit_LookDepthIsObeyed()
        {
            //arrange
            //we set up a folder with an identical named subfolder with dummy content
            const string testDirName = "ConsolidateTestDir_ForLookDepthLimit";

            var rootTestdir = TestRoot.Dir.Add(testDirName);
            rootTestdir.CleanIfExists();

            var currentDir = CreateIdenticalSubdirs(rootTestdir, 2);
            AddTestContent(currentDir, 3, 1);

            GetHierarchyDepth(rootTestdir).Should().Be(3);//verify that we have a 3 level deep hierarchy

            //act
            rootTestdir.ConsolidateIdenticalSubfolders(1);

            //assert
            GetHierarchyDepth(rootTestdir).Should().Be(2);
            rootTestdir.GetDirectories().Count().Should().Be(1);
            rootTestdir.GetFiles().Count().Should().Be(0);
        }
        
        [Fact]
        public void ConsolidateIdenticalSubfolders_IgnoreCaseWhenConsolidating_IdenticalSubfoldersAreConsolidated()
        {
            //arrange
            //we set up a folder with an identical named subfolder with dummy content
            const string testDirName = "ConsolidateTestDir_WithReallyLongName";

            var rootTestdir = TestRoot.Dir.Add(testDirName);
            rootTestdir.CleanIfExists();

            var currentDir = CreateIdenticalSubdirs(rootTestdir, 3);

            const int numOfTestDirs = 3;

            AddTestContent(currentDir, 3, 1);

            currentDir.GetDirectories().Count().Should().Be(numOfTestDirs);
            currentDir.GetFiles().Count().Should().Be(1);

            //act
            rootTestdir.ConsolidateIdenticalSubfolders();

            //assert
            rootTestdir.GetDirectories().Count().Should().Be(numOfTestDirs);
            rootTestdir.GetFiles().Count().Should().Be(1);
            rootTestdir.GetDirectories(testDirName).Count().Should().Be(0);//the identical named subfolder should be gone
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
