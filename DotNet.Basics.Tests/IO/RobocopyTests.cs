using System.Linq;
using DotNet.Basics.IO;
using DotNet.Basics.Tests.IO.Testa;
using DotNet.Basics.TestsRoot;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.IO
{
    public class RobocopyTests : TestWithHelpers
    {
        public RobocopyTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void MoveContent_TargetFolderDoesntExist_SourceFolderIsMoved()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var emptyDir = testDir.ToDir("empty");
                var sourceDir = testDir.ToDir("source");
                var targetDir = testDir.ToDir("target");
                TestFile1 testSource = null;
                WithTestRoot(testRoot => testSource = new TestFile1(testRoot));
                Robocopy.CopyDir(testSource.Directory().FullName(), sourceDir.FullName(), true, null);
                emptyDir.CreateIfNotExists();
                emptyDir.CleanIfExists();
                emptyDir.GetPaths().Count.Should().Be(0);//empty dir
                sourceDir.Exists().Should().BeTrue(sourceDir.FullName());
                targetDir.DeleteIfExists();
                targetDir.Exists().Should().BeFalse(targetDir.FullName());

                //act
                Robocopy.MoveContent(sourceDir.FullName(), targetDir.FullName(), null, true, null);
                Robocopy.MoveContent(emptyDir.FullName(), targetDir.FullName(), null, true, null);//move empty dir to ensure target dir is not cleaned

                //assert
                sourceDir.Exists().Should().BeTrue(sourceDir.FullName());
                sourceDir.IsEmpty();
                targetDir.GetFiles().Single().Name.Should().Be(testSource.Name);
            });
        }

        [Fact]
        public void Copy_CopySingleFileSourceExists_FileIsCopied()
        {
            ArrangeActAssertPaths(testDir =>
            {
                TestFile1 sourceFile = null;
                WithTestRoot(testRoot => sourceFile = new TestFile1(testRoot));
                sourceFile.Exists().Should().BeTrue("source file should exist");

                var targetFile = testDir.ToFile("Copy_CopySingleFileSourceExists_FileIsCopied", sourceFile.Name);
                targetFile.DeleteIfExists();
                targetFile.Exists().Should().BeFalse("target file should not exist before copy");

                //act
                var result = Robocopy.CopyFile(sourceFile.Directory().FullName(), targetFile.Directory().FullName(), sourceFile.Name);

                result.ExitCode.Should().BeLessThan(8); //http://ss64.com/nt/robocopy-exit.html
                targetFile.Exists().Should().BeTrue("target file is copied");
            });
        }

        [Fact]
        public void CopyDir_CopyDirSourceExists_DirIsCopied()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var source = testDir.ToDir("source");
                var target = testDir.ToDir("target");
                var sourceFile = source.ToFile("myfile.txt");
                var targetFile = source.ToFile(sourceFile.Name);
                var fileContent = "blavlsavlsdglsdflslfsdlfsdlfsd";
                target.DeleteIfExists();
                target.Exists().Should().BeFalse();

                source.CreateIfNotExists();
                sourceFile.WriteAllText(fileContent);
                sourceFile.Exists().Should().BeTrue();

                //act
                var result = Robocopy.CopyDir(source.FullName(), target.FullName(), true);

                //assert
                result.ExitCode.Should().BeLessThan(8); //http://ss64.com/nt/robocopy-exit.html
                target.Exists().Should().BeTrue();
                targetFile.Exists().Should().BeTrue();
                targetFile.ReadAllText().Should().Be(fileContent);
            });
        }
    }
}
