using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.IO
{
    [TestFixture]
    public class PathExtensionsTests
    {
        [Test]
        [TestCase(PathDelimiter.Slash)]
        [TestCase(PathDelimiter.Backslash)]
        public void ToPath_PathDelimiter_DelimiterIsSet(PathDelimiter pathDelimiter)
        {
            var pathPt1 = "pt1";
            var pathPt2 = "pt2";

            var path = pathPt1.ToPath(pathDelimiter, pathPt2);
            var pathRef = pathPt1.ToPath(pathPt2).WithDelimiter(pathDelimiter);
            var delimiter = '/';
            if (pathDelimiter == PathDelimiter.Backslash)
                delimiter = '\\';

            path.Should().Be(pathPt1 + delimiter + pathPt2);
            path.Should().Be(pathRef);
        }
    }
}
