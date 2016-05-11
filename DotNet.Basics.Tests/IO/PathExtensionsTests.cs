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

        [Test]
        [TestCase("pt101", "pt2")]//file
        [TestCase("pt101", "pt2")]//file
        public void ToIoPath_Combine_PathIsGenerated(string pt1, string pt2)
        {
            var expectedPath = pt1 + PathExtensions.BackslashDelimiter + pt2;

            var path = pt1.ToIoPath(pt2);

            path.Should().Be(expectedPath);
        }

        [Test]
        [TestCase("pt101", "pt2")]//file
        [TestCase("pt101", "pt2")]//file
        public void ToUriPath_Combine_PathIsGenerated(string pt1, string pt2)
        {
            var expectedPath = pt1 + PathExtensions.SlashDelimiter + pt2;

            var path = pt1.ToUriPath(pt2);

            path.Should().Be(expectedPath);
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
