using System;
using System.IO;
using System.Linq;
using DotNet.Basics.IO;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.IO
{
    [TestFixture]
    public class IoDirTests
    {
        private const string _testDirRoot = @"K:\testDir";
        private const string _testDoubleDir = @"\testa\testb";
        private const string _testSingleDir = @"\testk\";

        [Test]
        public void DeleteIfExists_DirExists_DirIsDeleted()
        {
            var dir = @"DeleteIfExists_DirExists_DirIsDeleted".ToDir();

            dir.CreateIfNotExists();

            dir.Exists().Should().BeTrue();

            //act
            dir.DeleteIfExists();

            dir.Exists().Should().BeFalse();
        }


        [Test]
        public void DeleteIfExists_DeleteLongNamedDir_DirIsDeleted()
        {
            //arrange
            //we set up a folder with a really long name
            const string testDirName = "DeleteIfExists_DeleteLongNamedDir_DirIsDeleted";

            var rootTestdir = new IoDir(@".\", testDirName);
            rootTestdir.CleanIfExists();
            var identicalSubDir = rootTestdir;
            try
            {
                //we keep adding sub folders until we reach our limit - whichever comes first
                for (var level = 0; level < 100; level++)
                {
                    identicalSubDir.CreateIfNotExists();
                    identicalSubDir = identicalSubDir.ToDir(testDirName);
                }
            }
            catch (PathTooLongException)
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



        [Test]
        public void Parent_NameOnlySourceDir_PartenIsResolved()
        {
            var currentDir = new DirectoryInfo(@".");
            var dir = @"Parent_ParentDir_PartenIsResolved".ToDir();

            dir.Parent.FullName.Should().Be(currentDir.FullName);
        }

        [Test]
        public void Parent_FullSourceDir_PartenIsResolved()
        {
            var parent = @"C:\Projects\Cloud.Services.SitecoreAsAService.CargoContainer\Code\Sitecore.Cloud.CargoContainer.Tests\bin\Debug".ToDir();
            var dir = parent.ToDir("Parent_ParentDir_PartenIsResolved");

            dir.Parent.FullName.Should().Be(parent.FullName);
        }


        [Test]
        public void CopyTo_IncludeSubDirectories_DirIsCopied()
        {
            const int dirDepth = 3;
            var root = "cpyTo_InclSubDirs_DirIsCop".ToDir();
            root.DeleteIfExists();
            var currentDir = CreateIdenticalSubdirs(root, dirDepth);
            "blaaaa".WriteToDisk(currentDir, "myFile.txt");

            var targetRoot = (root.Name + "Target").ToDir();
            var targetDir = targetRoot.CreateSubdir(root.Name);
            targetDir.DeleteIfExists();
            targetDir.Exists().Should().BeFalse();

            root.CopyTo(targetDir, DirCopyOptions.IncludeSubDirectories);

            targetDir.Exists().Should().BeTrue();
            GetHierarchyDepth(targetDir).Should().Be(dirDepth + 1);
        }

        [Test]
        public void ConsolidateIdenticalSubfolders_LookDepthLimit_LookDepthIsObeyed()
        {
            //arrange
            //we set up a folder with an identical named subfolder with dummy content
            const string testDirName = "ConsolidateTestDir_ForLookDepthLimit";

            var rootTestdir = new IoDir(@".\", testDirName);
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

        private int GetHierarchyDepth(IoDir root)
        {
            var subDir = root.GetDirectories().SingleOrDefault(dir => dir.Name.Equals(root.Name, StringComparison.InvariantCultureIgnoreCase));
            if (subDir == null)
                return 1;
            var subIoDir = subDir.ToDir();
            return GetHierarchyDepth(subIoDir) + 1;
        }


        [Test]
        public void ConsolidateIdenticalSubfolders_IgnoreCaseWhenConsolidating_IdenticalSubfoldersAreCOnsolidated()
        {
            //arrange
            //we set up a folder with an identical named subfolder with dummy content
            const string testDirName = "ConsolidateTestDir_WithReallyLongName";

            var rootTestdir = new IoDir(@".\" + testDirName);
            rootTestdir.CleanIfExists();

            var currentDir = CreateIdenticalSubdirs(rootTestdir, 10);

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

        [Test, Ignore("depends on rogue folder")]
        public void ConsolidateIdenticalSubfolders()
        {
            //arrange
            //we set up a folder with an identical named subfolder with dummy content
            var rootTestdir = new IoDir(@"C:\Users\rar\AppData\Local\dftmp\Resources\e59b639c-1cde-479a-a572-389619020a60\directory\ScaaSTemp\Sitecore 7.5 rev. 140612_Andes");

            //act
            rootTestdir.ConsolidateIdenticalSubfolders();

            //assert
            rootTestdir.GetDirectories().Count().Should().Be(3);
            rootTestdir.GetFiles().Count().Should().Be(0);
            rootTestdir.GetDirectories(rootTestdir.Name).Count().Should().Be(0);//the identical named subfolder should be gone
        }

        [Test]
        public void CreateIfExists_CreateOptions_ExistingDirIsCleaned()
        {
            //arrange
            var testDir = new IoDir(@"CreateIfExists_CreateOptions_ExistingDirIsCleaned");
            @"bllll".WriteToDisk(testDir, "myFile.txt");

            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Count().Should().Be(1);

            //act
            testDir.CreateIfNotExists(DirCreateOptions.CleanIfExists);

            //assert
            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Count().Should().Be(0);
        }

        [Test]
        public void CreateIfExists_CreateOptions_ExistingDirIsNotCleaned()
        {
            //arrange
            var testDir = new IoDir(@"CreateIfExists_CreateOptions_ExistingDirIsNotCleaned");
            testDir.DeleteIfExists();
            @"bllll".WriteToDisk(testDir, "myFile.txt");

            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Count().Should().Be(1);

            //act
            testDir.CreateIfNotExists(DirCreateOptions.DontCleanIfExists);

            //assert
            testDir.Exists().Should().BeTrue();
            testDir.GetFiles().Count().Should().Be(1);
        }

        [Test]
        public void CleanIfExists_DirDoesntExists_NoActionAndNoExceptions()
        {
            //arrange
            var testDir = new IoDir(@"SOMETHINGTHAT DOESNT EXIST_BLAAAAAA");

            //act
            testDir.CleanIfExists();

            //assert
            testDir.Exists().Should().BeFalse();
        }
        [Test]
        public void CleanIfExists_RemoveAllContentFromADir_DirIsCleaned()
        {
            //arrange
            var testDir = new IoDir(@".\MyTestDir");
            testDir.CreateIfNotExists();

            const int numOfTestFiles = 3;

            AddTestContent(testDir, 0, numOfTestFiles);

            testDir.GetFiles().Count().Should().Be(numOfTestFiles);

            //act
            testDir.CleanIfExists();

            testDir.GetFiles().Count().Should().Be(0);
        }

        [Test]
        public void ToDir_CombineToDir_FullNameIsCorrect()
        {
            var actualDir = _testDirRoot.ToDir(_testDoubleDir);
            const string expected = _testDirRoot + _testDoubleDir;

            actualDir.FullName.Should().Be(expected);
        }

        [Test]
        public void ToDir_CombineToDirectoryInfo_FullNameIsCorrect()
        {
            var actual = _testDirRoot.ToDir(_testDoubleDir);
            var expected = _testDirRoot + _testDoubleDir;
            actual.FullName.Should().Be(expected);


            actual = new IoDir(@"c:\BuildLibrary\Sitecore\Sitecore 8.0 rev. 141212", "Website");
            expected = @"c:\BuildLibrary\Sitecore\Sitecore 8.0 rev. 141212\Website";
            actual.FullName.Should().Be(expected);
        }

        [Test]
        public void ToDir_ParentFromCombine_ParentFolderOfNewIsSameAsOrgRoot()
        {
            var rootDir = _testDirRoot.ToDir();
            var dir = rootDir.ToDir(_testSingleDir);
            dir.Parent.Name.Should().Be(rootDir.Name);
        }

        private void AddTestContent(IoDir dir, int numOfTestDirs, int numOfTestFiles)
        {
            for (var i = 0; i < numOfTestDirs; i++)
            {
                var testFolder = dir.ToDir("MyTestFolder" + i);
                testFolder.CreateIfNotExists();
                "blaaa".WriteToDisk(testFolder, $"blaa{i}.txt");
            }
            for (var i = 0; i < numOfTestFiles; i++)
                "blaaaaa".WriteToDisk(dir, $"myFile{i}.txt");
        }


        private IoDir CreateIdenticalSubdirs(IoDir root, int maxDepth)
        {
            try
            {
                //we keep adding sub folders until we reach our limit or ten levels - whichever comes first
                for (var level = 0; level < maxDepth; level++)
                {
                    var dirName = level % 2 == 0 ? root.Name.ToLower() : root.Name.ToUpper();
                    root = root.CreateSubdir(dirName);
                }
            }
            catch (PathTooLongException)
            {
                //we set it to one level up so were sure we can have test content
                root = root.Parent;
                root.CleanIfExists();
            }
            return root;
        }
    }
}
