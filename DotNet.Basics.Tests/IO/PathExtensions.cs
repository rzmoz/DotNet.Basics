using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.IO
{
    [TestFixture]
    public class PathExtensions
    {
        [Test]
        [TestCase(PathDelimiter.Slash)]
        [TestCase(PathDelimiter.Backslash)]
        public void ToPath_PathDelimiter_DelimiterIsSet(PathDelimiter pathDelimiter)
        {
            var pathPt1 = "pt1";
            var pathPt2 = "pt2";

            var path = pathPt1.ToPath(pathPt2).WithDelimiter(pathDelimiter);

            var delimiter = '/';
            if (pathDelimiter == PathDelimiter.Backslash)
                delimiter = '\\';

            path.Should().Be(pathPt1 + delimiter + pathPt2);
        }
    }
}
