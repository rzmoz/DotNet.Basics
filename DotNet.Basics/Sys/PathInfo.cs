using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using DotNet.Basics.IO;
using DotNet.Basics.Sys.Text;

namespace DotNet.Basics.Sys
{
    public abstract class PathInfo
    {
        private static readonly SysRegex _windowsRootPathRegex = @"^[a-zA-X]:";
        private const string _uriSchemePattern = @"^([a-zA-Z]+://)";
        public const char Slash = '/';

        private static readonly Regex _uriSchemeRegex =
            new(_uriSchemePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private const string _uncDetector = @"\\";

        protected PathInfo(string path, PathType pathType, params string[] segments)
        {
            path ??= string.Empty;

            var flattened = Flatten(path, segments);

            var isUnc = false;
            string uriScheme = null;

            if (flattened.Any())
            {
                isUnc = flattened.First().StartsWith(_uncDetector);
                var uriMatch = _uriSchemeRegex.Match(flattened.First());
                if (uriMatch.Success)
                    uriScheme = uriMatch.Groups[0].Value;
            }

            //Clean and tokenize segments
            Segments = Tokenize(flattened);
            PathType = pathType == PathType.Unknown ? DetectPathType(path, segments) : pathType;

            //Set raw path
            RawPath = Flatten(flattened, Path.IsPathRooted(path));
            RawPath = ConformPathSeparator(RawPath);
            if (isUnc)
                RawPath = RawPath.EnsurePrefix(_uncDetector);
            else if (uriScheme != null)
                RawPath = RawPath.RemovePrefix(uriScheme.TrimEnd('/')).TrimStart('/').EnsurePrefix(uriScheme);
            if (Path.IsPathRooted(path))
                RawPath = RawPath.EnsurePrefix(Slash);

            Parent = null;
            var parentSegments = Segments.Take(Segments.Count - 1).ToArray();
            if (parentSegments.Any())
            {
                if (Path.IsPathRooted(path) && !_windowsRootPathRegex.IsMatch(path))
                    Parent = new DirPath("/", parentSegments);
                else
                    Parent = new DirPath(null, parentSegments);
            }

            //set name
            Name = Path.GetFileName(RawPath.RemoveSuffix(Slash));
        }

        public string RawPath { get; }
        public string Name { get; }

        [JsonIgnore] [IgnoreDataMember] public string NameWoExtension => Path.GetFileNameWithoutExtension(Name);
        [JsonIgnore] [IgnoreDataMember] public string Extension => Path.GetExtension(Name);
        [JsonIgnore] [IgnoreDataMember] public string FullName => Path.GetFullPath(RawPath);

        public PathType PathType { get; }

        [JsonIgnore] [IgnoreDataMember] public DirPath Parent { get; }

        [JsonIgnore] [IgnoreDataMember] public DirPath Directory => PathType == PathType.File ? Parent : (DirPath)this;

        public IReadOnlyCollection<string> Segments;

        public static List<string> Tokenize(ICollection<string> segments)
        {
            var tokens = new List<string>();
            foreach (var segment in segments)
                tokens.AddRange(Tokenize(segment));
            return tokens;
        }

        private static IEnumerable<string> Tokenize(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return Enumerable.Empty<string>();

            path = ConformPathSeparator(path);

            return path.Split(Slash, StringSplitOptions.RemoveEmptyEntries)
                .Where(seg => string.IsNullOrWhiteSpace(seg) == false);
        }

        public static IList<string> Flatten(string path, ICollection<string> segments)
        {
            var asOne = new List<string>();
            if (string.IsNullOrWhiteSpace(path) == false)
                asOne.Add(ConformPathSeparator(path));
            asOne.AddRange(segments.Where(seg => string.IsNullOrWhiteSpace(seg) == false).Select(ConformPathSeparator));
            return asOne;
        }

        public static string Flatten(ICollection<string> segments, bool isRooted)
        {
            var tokenized = Tokenize(segments);
            var flattened = tokenized.JoinString(Slash.ToString());

            if (isRooted)
                flattened = flattened.EnsurePrefix(Slash);
            return flattened;
        }


        public static PathType DetectPathType(string path, string[] segments)
        {
            var lookingAt = path;
            if (segments.Length > 0)
                lookingAt = segments.Last();

            if (lookingAt == null)
                return PathType.Unknown;

            if (Path.GetExtension(lookingAt.TrimEnd(Slash)).Length > 0)
                return PathType.File;
            return lookingAt.EndsWith(Slash.ToString())
                ? PathType.Dir
                : PathType.File;
        }

        public static string ConformPathSeparator(string path)
        {
            return path.Replace('\\', Slash);
        }

        public static bool IsWindowsRooted(string path) => _windowsRootPathRegex.IsMatch(path);

        public override string ToString()
        {
            return RawPath;
        }

        protected bool Equals(PathInfo other)
        {
            return RawPath.Equals(other.RawPath,
                StringComparison
                    .OrdinalIgnoreCase); //ignore case on comparison since we're mainly Windows. To bad (L)inux systems
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PathInfo)obj);
        }

        public override int GetHashCode()
        {
            return RawPath.GetHashCode();
        }
    }
}