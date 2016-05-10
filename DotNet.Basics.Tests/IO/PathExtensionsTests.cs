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
        [TestCase("With\\Backslash", 2)]
        [TestCase("With/slash", 2)]
        [TestCase("NoDelimiter", 1)]
        [TestCase("", 0)]//empty
        [TestCase(null, 0)]//empty
        public void ToPathTokens_Tokenize_PathIsSplit(string path, int expectedNoOfTokens)
        {
            var tokens = path.ToPathTokens();
            tokens.Count.Should().Be(expectedNoOfTokens);

            if (string.IsNullOrEmpty(path) == false)
                path.Should().StartWith(tokens.First());
        }

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
