using System.Linq;
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
        [TestCase("With\\Backslash\\", 2)]//dir
        [TestCase("With\\Backslash", 2)]//file
        [TestCase("With/slash", 2)]
        [TestCase("NoDelimiter", 1)]
        [TestCase("          ", 1)]//spaces
        [TestCase("", 0)]//empty
        [TestCase(null, 0)]//empty
        public void ToPathTokens_Tokenize_PathIsSplit(string path, int expectedNoOfTokens)
        {
            var tokens = path.ToPathTokens();
            tokens.Count.Should().Be(expectedNoOfTokens);

            if (string.IsNullOrEmpty(path) == false)
            {
                path.Should().StartWith(tokens.First());
                path.Should().EndWith(tokens.Last());
            }
        }

        [Test]
        [TestCase("//pt101", "pt2", PathDelimiter.Slash)]//file
        [TestCase("\\pt101", "pt2", PathDelimiter.Backslash)]//file
        [TestCase("//pt101", "pt2//", PathDelimiter.Slash)]//dir
        [TestCase("\\pt101", "pt2\\", PathDelimiter.Backslash)]//dir
        public void ToPath_Combine_PathIsGenerated(string pt1, string pt2, PathDelimiter pathDelimiter)
        {
            var path = pt1.ToPath(pathDelimiter, pt2);
            var pathRef = pt1.ToPath(pt2).WithDelimiter(pathDelimiter);

            path.Should().Be(pt1 + GetDelimiter(pathDelimiter) + pt2);
            path.Should().Be(pathRef);
        }

        private char GetDelimiter(PathDelimiter pathDelimiter)
        {
            var delimiter = '/';
            if (pathDelimiter == PathDelimiter.Backslash)
                delimiter = '\\';
            return delimiter;
        }
    }
}
