using DotNet.Basics.Sys;
using DotNet.Basics.TestsRoot;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.IO.Robust.Tests
{
    public class LongPathTests : TestWithHelpers
    {
        public LongPathTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void CreateDirectory_VeryLongPaths_DirIsCreated()
        {
            var veryLongPath = TestRoot.ToDir("CreateDirectory_VeryLongPaths_NoExceptionIsThrown", @"asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd");
            veryLongPath.DeleteIfExists();
            veryLongPath.Exists().Should().BeFalse();
            //act
            LongPath.CreateDir(veryLongPath.FullName());
            //assert
            veryLongPath.Exists().Should().BeTrue();
        }

        [Fact]
        public void Exists_VeryLongPaths_NoExceptionIsThrown()
        {
            var veryLongPath = @"c:\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd";
            //act
            var result = LongPath.Exists(veryLongPath);
            //assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GetfullName_VeryLongPaths_FullNameIsResolved()
        {
            var veryLongPath = @"asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd";
            //act
            var result = LongPath.GetFullName(veryLongPath);
            //assert
            result.Should().Be($@"{TestRoot}\{veryLongPath}");
        }

        [Fact]
        public void TryDelete_DeleteDir_DirIsDeleted()
        {
            var veryLongPath = TestRoot.ToDir(@"asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd\asdasd");
            veryLongPath.CreateIfNotExists();
            veryLongPath.Exists().Should().BeTrue();
            //act
            var result = LongPath.TryDelete(veryLongPath.FullName());
            //assert
            result.Should().BeTrue();
            veryLongPath.Exists().Should().BeFalse();
        }


    }
}
