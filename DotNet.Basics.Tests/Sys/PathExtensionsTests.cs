using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class PathExtensionsTests
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
        public void Add_File_SameTypeIsReturned()
        {
            var dir = _path.ToFile().Add(_segment);

            dir.Should().BeOfType<FilePath>();
            dir.RawPath.Should().Be(_path + $"/{_segment }");
        }

        [Fact]
        public void ToDir_Create_DirIsCreated()
        {
            var dir = _path.ToDir().ToFile().ToDir(_segment);//different extension methods

            dir.Should().BeOfType<DirPath>();
            dir.RawPath.Should().Be(_path + $"/{_segment }/");
        }

        [Fact]
        public void ToFile_Create_FileIsCreated()
        {
            var file = _path.ToFile().ToDir().ToFile(_segment);//different extension methods

            file.Should().BeOfType<FilePath>();
            file.RawPath.Should().Be(_path + $"/{_segment }");
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
    }
}
