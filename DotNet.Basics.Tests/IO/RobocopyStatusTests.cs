using DotNet.Basics.IO;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.IO
{
    public class RobocopyStatusTests
    {
        [Theory]
        [InlineData(0, false)]
        [InlineData(1, false)]
        [InlineData(2, false)]
        [InlineData(3, false)]
        [InlineData(4, false)]
        [InlineData(5, false)]
        [InlineData(6, false)]
        [InlineData(7, false)]

        [InlineData(8, true)]
        [InlineData(9, true)]
        [InlineData(10, true)]
        [InlineData(11, true)]
        [InlineData(12, true)]
        [InlineData(13, true)]
        [InlineData(14, true)]
        [InlineData(15, true)]
        [InlineData(16, true)]
        public void Failed_FailedWhen8OrLarger_FailedIsRight(int statusCode, bool expectedFailed)
        {
            var status = new RobocopyStatus(statusCode);

            status.Failed.Should().Be(expectedFailed);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(17)]
        public void ExitCode_UnknownStatusCode_CodeIsSetToFatalError(int statusCode)
        {
            var status = new RobocopyStatus(statusCode);

            //and should not throw exceptions
            status.ExitCode.Should().Be(statusCode);
            status.Failed.Should().BeTrue();
            
        }
    }
}
