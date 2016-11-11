using System;
using System.Linq;
using DotNet.Basics.IO;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.IO
{

    public class DirExtensionsTests
    {
        private const string _testDirRoot = @"K:\testDir";
        private const string _testDoubleDir = @"\testa\testb";
        private const string _testSingleDir = @"\testk\";

        [Theory]
        [InlineData("http://localhost/", "myDir/")] //http dir
        [InlineData("https://localhost/", "myDir/")] //https dir
        public void FullName_Uri_ParsedPathIsUri(string uri, string segment)
        {
            var path = uri.ToDir(segment);
            Action action = () => new Uri(path.FullName);
            action.ShouldNotThrow<UriFormatException>(path.FullName);
            path.FullName.Should().Be(uri + segment);
        }

        [Fact]
        public void DeleteIfExists_DirExists_DirIsDeleted()
        {
            var dir = @"DeleteIfExists_DirExists_DirIsDeleted".ToDir();
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

            var rootTestdir = ".".ToDir(testDirName);
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
        public void Parent_NameOnlySourceDir_PartenIsResolved()
        {
            var currentDir = ".".ToDir();
            var dir = currentDir.FullName.ToDir(@"Parent_NameOnlySourceDir_PartenIsResolved");

            dir.Parent.FullName.Should().Be(currentDir.FullName);
        }

        [Fact]
        public void Parent_FullSourceDir_ParenItsResolved()
        {
            var parent = ".".ToDir();
            var subCir = parent.ToDir("Parent_ParentDir_PartenIsResolved");

            subCir.Parent.FullName.Should().Be(parent.FullName);
        }


        [Fact]
        public void CopyTo_IncludeSubDirectories_DirIsCopied()
        {
            const int dirDepth = 3;
            var root = @"CopyTo_InclSubDirs_DirIsCop".ToDir();
            root.DeleteIfExists();
            var currentDir = CreateIdenticalSubdirs(root, dirDepth);
            "blaaaa".WriteAllText(currentDir.ToFile("myFile.txt"));

            var targetRoot = ".".ToDir(root.Name + "Target");
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

            var rootTestdir = ".".ToDir(testDirName);
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

        private int GetHierarchyDepth(DirPath root)
        {
            var subDir = root.GetDirectories().SingleOrDefault(dir => dir.Name.Equals(root.Name, StringComparison.InvariantCultureIgnoreCase));
            if (subDir == null)
                return 1;
            var subIoDir = subDir.ToDir();
            return GetHierarchyDepth(subIoDir) + 1;
        }


        [Fact]
        public void ConsolidateIdenticalSubfolders_IgnoreCaseWhenConsolidating_IdenticalSubfoldersAreCOnsolidated()
        {
            //arrange
            //we set up a folder with an identical named subfolder with dummy content
            const string testDirName = "ConsolidateTestDir_WithReallyLongName";

            var rootTestdir = ".".ToDir(testDirName);
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

        [Fact]
        public void CreateIfExists_CreateOptions_ExistingDirIsCleaned()
        {
            //arrange
            var testDir = @"CreateIfExists_CreateOptions_ExistingDirIsCleaned".ToDir();
            @"bllll".WriteAllText(testDir.ToFile("myFile.txt"));

            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Count().Should().Be(1);

            //act
            testDir.CreateIfNotExists();
            testDir.CleanIfExists();

            //assert
            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Count().Should().Be(0);
        }

        [Fact]
        public void CreateIfExists_CreateOptions_ExistingDirIsNotCleaned()
        {
            //arrange
            var testDir = @"CreateIfExists_CreateOptions_ExistingDirIsNotCleaned".ToDir();
            testDir.DeleteIfExists();
            @"bllll".WriteAllText(testDir.ToFile("myFile.txt"));

            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Count().Should().Be(1);

            //act
            testDir.CreateIfNotExists();

            //assert
            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Count().Should().Be(1);
        }

        [Fact]
        public void CleanIfExists_DirDoesntExists_NoActionAndNoExceptions()
        {
            //arrange
            var testDir = @"SOMETHINGTHAT DOESNT EXIST_BLAAAAAA".ToDir();

            //act
            testDir.CleanIfExists();

            //assert
            testDir.Exists().Should().BeFalse();
        }
        [Fact]
        public void CleanIfExists_RemoveAllContentFromADir_DirIsCleaned()
        {
            //arrange
            var testDir = @"CleanIfExists_RemoveAllContentFromADir_DirIsCleaned".ToDir();
            testDir.CreateIfNotExists();

            const int numOfTestFiles = 3;

            AddTestContent(testDir, 0, numOfTestFiles);

            testDir.GetFiles().Count().Should().Be(numOfTestFiles);

            //act
            testDir.CleanIfExists();

            testDir.GetFiles().Count().Should().Be(0);
        }

        [Fact]
        public void ToDir_CombineToDir_FullNameIsCorrect()
        {
            var actual = _testDirRoot.ToDir(_testDoubleDir);
            var expected = _testDirRoot + _testDoubleDir + actual.Delimiter.ToChar();
            actual.FullName.Should().Be(expected);


            actual = @"c:\BuildLibrary\Folder\Module 2.0.1".ToDir("Website");
            expected = @"c:\BuildLibrary\Folder\Module 2.0.1\Website\";
            actual.FullName.Should().Be(expected);
        }

        [Fact]
        public void ToDir_ParentFromCombine_ParentFolderOfNewIsSameAsOrgRoot()
        {
            var rootDir = _testDirRoot.ToDir();
            var dir = rootDir.ToDir(_testSingleDir);
            dir.Parent.Name.Should().Be(rootDir.Name);
        }

        private void AddTestContent(DirPath dir, int numOfTestDirs, int numOfTestFiles)
        {
            for (var i = 0; i < numOfTestDirs; i++)
            {
                var testFolder = dir.ToDir("MyTestFolder" + i);
                testFolder.CreateIfNotExists();
                "blaaa".WriteAllText(testFolder.ToFile($"blaa{i}.txt"));
            }
            for (var i = 0; i < numOfTestFiles; i++)
                "blaaaaa".WriteAllText(dir.ToFile($"myFile{i}.txt"));
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
