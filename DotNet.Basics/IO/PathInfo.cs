using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DotNet.Basics.Sys;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DotNet.Basics.IO
{
    public class PathInfo
    {
        private static readonly string _protocolPattern = @"^([a-zA-Z]+://)";
        private static readonly Regex _protocolRegex = new Regex(_protocolPattern, RegexOptions.Compiled);

        private readonly Func<string> _resolveFullName;

        public PathInfo()
            : this("")
        { }

        public PathInfo(string path, params string[] segments)
            : this(Join(path, segments))
        { }

        public PathInfo(IReadOnlyCollection<string> segments)
            : this(segments, DetectIsFolder(segments), DetectDelimiter(segments))
        { }

        public PathInfo(string path, IReadOnlyCollection<string> segments, bool isFolder, PathDelimiter delimiter)
            : this(Join(path, segments), isFolder, delimiter)
        { }

        public PathInfo(string path, bool isFolder, PathDelimiter delimiter)
            : this(Join(path), isFolder, delimiter)
        { }

        public PathInfo(IReadOnlyCollection<string> segments, bool isFolder, PathDelimiter delimiter)
        {
            Segments = FlattenSegments(segments);
            IsFolder = isFolder;
            //detect IsUri
            IsUri = DetectIsUri(segments);
            Delimiter = IsUri ? PathDelimiter.Slash : delimiter;

            if (IsUri)
                _resolveFullName = () => RawName;
            else
                _resolveFullName = () => SystemIoPath.GetFullPath(RawName);

            //init name
            Name = Segments.LastOrDefault() ?? string.Empty;
            NameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(Name);
            Extension = System.IO.Path.GetExtension(Name);
        }

        public string Name { get; }
        public bool IsUri { get; }
        public bool IsFolder { get; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PathDelimiter Delimiter { get; }
        public IReadOnlyCollection<string> Segments { get; }

        [JsonIgnore]
        public bool IsRooted => System.IO.Path.IsPathRooted(RawName);
        [JsonIgnore]
        public string FullName => _resolveFullName();
        [JsonIgnore]
        public string NameWithoutExtension { get; }
        [JsonIgnore]
        public string Extension { get; }
        [JsonIgnore]
        public string RawName => ToString(Delimiter);
        [JsonIgnore]
        public DirPath Directory => IsFolder ? new DirPath(RawName) : Parent;
        [JsonIgnore]
        public DirPath Parent => GetParent();

        private DirPath GetParent()
        {
            if (Segments.Count <= 1)
                return null;
            var parentSegments = Segments.Reverse().Skip(1).Reverse().ToArray();
            return new DirPath(parentSegments, Delimiter);
        }

        /// <summary>
        /// Returns a new Path where original and added paths are combined
        /// </summary>
        /// <param name="pathSegments"></param>
        /// <returns></returns>
        public PathInfo Add(params string[] pathSegments)
        {
            var combinedSegments = AddSegments(pathSegments);
            return new PathInfo(combinedSegments, IsFolder, Delimiter);
        }

        protected IReadOnlyCollection<string> AddSegments(params string[] pathSegments)
        {
            if (pathSegments == null)
                return Segments.ToArray();

            var splitNewSegments = FlattenSegments(pathSegments);
            if (splitNewSegments.Count == 0)
                return Segments.ToArray();

            var combinedSegments = Segments.Concat(pathSegments).ToArray();
            return combinedSegments;
        }

        public override string ToString()
        {
            return ToString(Delimiter);
        }

        public string ToString(PathDelimiter delimiter)
        {
            return GetRawName(Segments, IsFolder, delimiter);
        }

        protected bool Equals(PathInfo other)
        {
            return RawName == other.RawName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PathInfo)obj);
        }

        public override int GetHashCode()
        {
            return RawName.GetHashCode();
        }

        private static IReadOnlyCollection<string> Join(string path)
        {
            return new[] { path };
        }
        private static IReadOnlyCollection<string> Join(string path, IReadOnlyCollection<string> segments)
        {
            return string.IsNullOrWhiteSpace(path) ?
                segments :
                new[] { path }.Concat(segments).ToArray();
        }

        private static bool DetectIsFolder(IReadOnlyCollection<string> segments)
        {
            if (segments == null) throw new ArgumentNullException(nameof(segments));

            var ioPath = GetRawName(FlattenSegments(segments), true, PathDelimiter.Backslash);
            var fullName = SystemIoPath.GetFullPath(ioPath);
            if (SystemIoPath.Exists(fullName, true))
                return true;
            if (SystemIoPath.Exists(fullName, false))
                return false;

            return segments.Last().EndsWith(PathChars.Slash.ToString()) || segments.Last().EndsWith(PathChars.Backslash.ToString());
        }

        private static bool DetectIsUri(IEnumerable<string> segments)
        {
            return segments.FirstOrDefault(s =>
            s.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                        s.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                        s.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase) ||
                        s.StartsWith("sftp://", StringComparison.OrdinalIgnoreCase)) != null;
        }

        private static PathDelimiter DetectDelimiter(IReadOnlyCollection<string> segments)
        {
            foreach (var segment in segments)
            {
                var slashIndex = segment.IndexOf(PathChars.Slash);
                var backslashIndex = segment.IndexOf(PathChars.Backslash);

                if (slashIndex < 0 && backslashIndex < 0)
                    continue;
                if (slashIndex < 0)
                    return PathDelimiter.Backslash;
                if (backslashIndex < 0)
                    return PathDelimiter.Slash;

                return (slashIndex < backslashIndex) ? PathDelimiter.Slash : PathDelimiter.Backslash;
            }
            return PathDelimiter.Backslash;
        }

        private static string GetRawName(IEnumerable<string> segments, bool isFolder = true, PathDelimiter delimiter = PathDelimiter.Slash)
        {
            var path = string.Empty;
            foreach (var segment in segments)
            {
                if (segment.IsProtocol())
                    path += segment;
                else
                    path += $"{segment}{delimiter.ToChar()}";
            }

            path = isFolder ? path : path.RemoveSuffix(delimiter.ToChar());

            return delimiter == PathDelimiter.Slash ?
                path.Replace(PathChars.Backslash, PathChars.Slash) :
                path.Replace(PathChars.Slash, PathChars.Backslash);
        }

        private static IReadOnlyCollection<string> FlattenSegments(IReadOnlyCollection<string> pathSegments)
        {
            if (pathSegments == null)
                return new string[0];

            var updatedSegments = new List<string>();
            foreach (var pathSegment in pathSegments)
            {
                if (string.IsNullOrWhiteSpace(pathSegment))
                    continue;

                var toBesplit = pathSegment;

                //extract protocol if present
                var protocolMatch = _protocolRegex.Match(toBesplit);
                if (protocolMatch.Success)
                {
                    var capture = protocolMatch.Captures[0].Value;

                    updatedSegments.Add(capture);
                    toBesplit = toBesplit.Substring(capture.Length);//remove protocol
                }

                var splits = toBesplit.Split(new[] { PathChars.Slash, PathChars.Backslash },
                    StringSplitOptions.RemoveEmptyEntries);
                updatedSegments.AddRange(splits);
            }

            return updatedSegments.ToArray();
        }
    }
}
