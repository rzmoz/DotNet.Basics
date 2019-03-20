using DotNet.Basics.IO;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.IO
{
    public class RobocopyStatusTests
    {
        [Theory]
        [InlineData(0, true)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        [InlineData(4, true)]
        [InlineData(5, true)]
        [InlineData(6, true)]
        [InlineData(7, true)]

        [InlineData(8, false)]
        [InlineData(9, false)]
        [InlineData(10, false)]
        [InlineData(11, false)]
        [InlineData(12, false)]
        [InlineData(13, false)]
        [InlineData(14, false)]
        [InlineData(15, false)]
        [InlineData(16, false)]
        public void Success_SuccessWhenLessThan8_SuccessIsRight(int statusCode, bool expectedSuccess)
        {
            var status = new RobocopyStatus(statusCode);

            status.Success.Should().Be(expectedSuccess);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(17)]
        public void ExitCode_UnknownStatusCode_CodeIsSetToFatalError(int statusCode)
        {
            var status = new RobocopyStatus(statusCode);

            status.StatusCode.Should().Be(16);
            status.Success.Should().BeFalse();
        }
    }
}
