using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Basics.IO;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.IO
{
    [TestFixture]
    public class PathTests
    {
        [Test]
        [TestCase("DetectDelimiter/", PathDelimiter.Slash)]//delimiter detected
        [TestCase("DetectDelimiter\\", PathDelimiter.Backslash)]//delimiter detected
        [TestCase("DetectDelimiter", PathDelimiter.Backslash)]//delimiter fallback
        public void Ctor_Delimiter_DelimiterDetected(string pathInput, PathDelimiter delimiter)
        {
            var path = new Path(pathInput);
            path.Delimiter.Should().Be(delimiter);
        }
    }
}
