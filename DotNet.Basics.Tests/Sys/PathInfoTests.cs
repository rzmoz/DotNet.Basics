using System.IO;
using System.Linq;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class PathInfoTests
    {
        [Theory]
        [InlineData(@"c:\my\path", @"c:\my\path", PathSeparator.Backslash)]//bs to bs
        [InlineData(@"c:\my\path", @"c:/my/path", PathSeparator.Slash)]//bs to bs
        [InlineData(@"c:/my/path", @"c:/my/path", PathSeparator.Slash)]//s to  s
        [InlineData(@"c:/my/path", @"c:\my\path", PathSeparator.Backslash)]//s to bs
        public void RawPath_PathSeparator_SeparatorIsOverridden(string path, string expected, PathSeparator separator)
        {
            //def path separator
            var pi = new PathInfo(path, IsFolder.Unknown, separator);
            pi.RawPath.Should().Be(expected);
        }

        [Theory]
        //files
        [InlineData(@"c:\my\path", @"c:\my\path")]//absolute single path
        [InlineData(@"my\path", @"\my\path")]//relative single path with starting delimiter
        [InlineData(@"my\path", @"my\path")]//relative single path without starting delimiter
        //dirs
        [InlineData(@"c:\my\path\", @"c:\my\path\")]//absolute single path
        [InlineData(@"my\path\", @"\my\path\")]//relative single path with starting delimiter
        [InlineData(@"my\path\", @"my\path\")]//relative single path without starting delimiter
        //segments
        [InlineData(@"c:\my\path\", @"c:\my\", @"\path\")]//absolute segmented path
        [InlineData(@"my\path\", @"\my\", @"\path\")]//relative segmented path with starting delimiter
        [InlineData(@"my\path\", @"my\", @"\path\")]//relative segmented path without starting delimiter
        public void RawPath_RawPathParsing_RawPathIsParsed(string expected, string path, params string[] segments)
        {
            //def path separator
            var pi = new PathInfo(path, segments);
            pi.RawPath.Should().Be(expected);

            //alt path separator
            var altPath = ToAlt(path);
            var altSegments = segments.Select(ToAlt).ToArray();
            var altExpected = ToAlt(expected);

            var altPathInfo = new PathInfo(altPath, altSegments);
            altPathInfo.RawPath.Should().Be(altExpected);
        }

        private string ToAlt(string p)
        {
            return p.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }
}

