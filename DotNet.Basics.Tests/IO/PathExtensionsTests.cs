using System.Linq;
using DotNet.Basics.IO;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.IO
{
    [TestFixture]
    public class PathExtensionsTests
    {
        [Test]
        [TestCase("//pt101", "pt2", PathDelimiter.Slash)]//file
        [TestCase("\\pt101", "pt2", PathDelimiter.Backslash)]//file
        [TestCase("//pt101", "pt2//", PathDelimiter.Slash)]//dir
        [TestCase("\\pt101", "pt2\\", PathDelimiter.Backslash)]//dir
        public void ToPath_Combine_PathIsGenerated(string pt1, string pt2, PathDelimiter pathDelimiter)
        {
            var path = pt1.ToPath(pathDelimiter, pt2);
            var pathRef = pt1.ToPath(pathDelimiter, pt2);

            path.Should().Be(pt1 + GetDelimiter(pathDelimiter) + pt2);
            path.Should().Be(pathRef);
        }

        private char GetDelimiter(PathDelimiter pathDelimiter)
        {
            var delimiter = PathExtensions.SlashDelimiter;
            if (pathDelimiter == PathDelimiter.Backslash)
                delimiter = PathExtensions.BackslashDelimiter;
            return delimiter;
        }
    }
}
