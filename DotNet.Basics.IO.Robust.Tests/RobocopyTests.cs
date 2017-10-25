using System.Linq;
using DotNet.Basics.IO.Robust.Tests.Testa;
using DotNet.Basics.Sys;
using DotNet.Basics.TestsRoot;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.IO.Robust.Tests
{
    public class RobocopyTests : TestWithHelpers
    {
        public RobocopyTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void MoveContent_TargetFolderDoesntExist_SourceFolderIsMoved()
        {
            var baseDir = TestRoot.ToDir("MoveContent_TargetFolderDoesntExist_SourceFolderIsMoved");
            var emptyDir = baseDir.ToDir("empty");
            var sourceDir = baseDir.ToDir("source");
            var targetDir = baseDir.ToDir("target");
            var testSource = new TestFile1();
            Robocopy.CopyDir(testSource.Directory().FullName(), sourceDir.FullName(), true, null);
            emptyDir.CreateIfNotExists();
            emptyDir.CleanIfExists();
            emptyDir.GetPaths().Length.Should().Be(0);//empty dir
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
        }

        [Fact]
        public void Copy_CopySingleFileSourceExists_FileIsCopied()
        {
            var sourcefile = new TestFile1();
            sourcefile.Exists().Should().BeTrue("source file should exist");

            var targetFile = TestRoot.ToFile("Copy_CopySingleFileSourceExists_FileIsCopied", sourcefile.Name);
            targetFile.DeleteIfExists();
            targetFile.Exists().Should().BeFalse("target file should not exist before copy");

            //act
            var result = Robocopy.CopyFile(sourcefile.Directory().FullName(), targetFile.Directory().FullName(), sourcefile.Name);

            result.ExitCode.Should().BeLessThan(8); //http://ss64.com/nt/robocopy-exit.html
            targetFile.Exists().Should().BeTrue("target file is copied");
        }

        [Fact]
        public void CopyDir_CopyDirSourceExists_DirIsCopied()
        {
            var source = TestRoot.ToDir("CopyDir_CopyDirSourceExists_DirIsCopied", "source");
            var target = TestRoot.ToDir("CopyDir_CopyDirSourceExists_DirIsCopied", "target");
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
        }
    }
}
