using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DotNet.Basics.Collections;

namespace DotNet.Basics.IO
{
    public class PathInfo
    {
        private readonly char _pathSeparatorChar;
        private readonly char _altPathSeparatorChar;

        public PathInfo(string path, params string[] segments)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            //detect separators
            var separators = DetectPathSeparator(path.ToArray(segments));
            _pathSeparatorChar = separators.Item1;
            _altPathSeparatorChar = separators.Item2;

            //Clean segments
            var cleanedSegments = CleanSegments(path, segments).ToArray();

            //set rawpath
            RawPath = string.Join(_pathSeparatorChar.ToString(), cleanedSegments);
        }

        public string RawPath { get; }

        public string FullName => RawPath;

        public bool Exists()
        {
            var fp = FullName;
            return File.Exists(fp) || Directory.Exists(fp);
        }

        public PathInfo Combine(params string[] segments)
        {
            return new PathInfo(RawPath, segments);
        }

        public override string ToString()
        {
            return FullName;
        }

        private IEnumerable<string> CleanSegments(string path, params string[] segments)
        {
            //to single string
            var joined = string.Join(_pathSeparatorChar.ToString(), path.ToArray(segments));
            //conform path separators
            joined = joined.Replace(_altPathSeparatorChar, _pathSeparatorChar);
            //remove duplicate path separators
            joined = Regex.Replace(joined, $@"[\{_pathSeparatorChar}]{{2,}}", _pathSeparatorChar.ToString(), RegexOptions.None);

            //check for explicit folder ending
            var isExplicitFolder = joined.EndsWith(_pathSeparatorChar.ToString());

            //to segments
            var split = joined.Split(new[] { _pathSeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

            //keep explicit folder
            if (isExplicitFolder)
                split[split.Length - 1] = split[split.Length - 1] + _pathSeparatorChar;
            return split;
        }

        private (char pathSeperator, char altPathSeperator) DetectPathSeparator(string[] segments)
        {
            var sepChar = Path.DirectorySeparatorChar;
            var altSepChar = Path.AltDirectorySeparatorChar;

            //if alt seperator does not outnumber default separator
            if (CharCount(segments, altSepChar) <= CharCount(segments, sepChar))
                return (sepChar, altSepChar);

            sepChar = Path.AltDirectorySeparatorChar;
            altSepChar = Path.DirectorySeparatorChar;
            return (sepChar, altSepChar);
        }

        private int CharCount(string[] segments, char @char)
        {
            return segments.Sum(segment => segment.Count(c => c == @char));
        }
    }
}
