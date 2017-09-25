using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Repeating;

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

        public PathInfo(string path, IReadOnlyCollection<string> segments, bool isFolder, char delimiter)
            : this(Join(path, segments), isFolder, delimiter)
        { }

        public PathInfo(string path, bool isFolder, char delimiter)
            : this(Join(path), isFolder, delimiter)
        { }

        public PathInfo(IReadOnlyCollection<string> segments, bool isFolder, char delimiter)
        {
            Segments = FlattenSegments(segments);
            IsFolder = isFolder;
            Delimiter = delimiter;
            _resolveFullName = () => SystemIoPath.GetFullPath(RawName);

            //init name
            Name = Segments.LastOrDefault() ?? string.Empty;
            NameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(Name);
            Extension = System.IO.Path.GetExtension(Name);
        }

        public string Name { get; }

        public bool IsFolder { get; }
        public char Delimiter { get; }
        public IReadOnlyCollection<string> Segments { get; }

        public bool IsRooted => System.IO.Path.IsPathRooted(RawName);
        public string FullName => _resolveFullName();
        public string NameWithoutExtension { get; }
        public string Extension { get; }
        public string RawName => ToString(Delimiter);
        public DirPath Directory => IsFolder ? new DirPath(RawName) : Parent;
        public DirPath Parent => GetParent();

        private DirPath GetParent()
        {
            if (Segments.Count <= 1)
                return null;
            var parentSegments = Segments.Reverse().Skip(1).Reverse().ToArray();
            return new DirPath(parentSegments, Delimiter);
        }

        public bool Exists(bool throwIoExceptionIfNotExists = false)
        {
            return SystemIoPath.Exists(FullName, IsFolder, throwIoExceptionIfNotExists);
        }

        public bool DeleteIfExists()
        {
            return DeleteIfExists(30.Seconds());
        }

        public bool DeleteIfExists(TimeSpan timeout)
        {

            if (Exists() == false)
                return true;

            Repeat.Task(() =>
            {
                PowerShellConsole.RemoveItem(FullName, force: true, recurse: true);
            })
            .WithOptions(o =>
            {
                o.Timeout = timeout;
                o.RetryDelay = 2.Seconds();
                o.DontRethrowOnTaskFailedType = typeof(ItemNotFoundException);
            })
            .Until(() => Exists() == false);

            return Exists() == false;
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

        public string ToString(char delimiter)
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

            return segments.Last().EndsWith(PathDelimiter.Slash.ToString()) || segments.Last().EndsWith(PathDelimiter.Backslash.ToString());
        }

        private static char DetectDelimiter(IReadOnlyCollection<string> segments)
        {
            foreach (var segment in segments)
            {
                var slashIndex = segment.IndexOf(PathDelimiter.Slash);
                var backslashIndex = segment.IndexOf(PathDelimiter.Backslash);

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

        private static string GetRawName(IEnumerable<string> segments, bool isFolder = true, char delimiter = PathDelimiter.Slash)
        {
            var path = string.Empty;
            foreach (var segment in segments)
            {
                if (segment.EndsWith("://"))//is protocol
                    path += segment;
                else
                    path += $"{segment}{delimiter}";
            }

            path = isFolder ? path : path.RemoveSuffix(delimiter);

            return delimiter == PathDelimiter.Slash ?
                path.Replace(PathDelimiter.Backslash, PathDelimiter.Slash) :
                path.Replace(PathDelimiter.Slash, PathDelimiter.Backslash);
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

                var splits = toBesplit.Split(new[] { PathDelimiter.Slash, PathDelimiter.Backslash },
                    StringSplitOptions.RemoveEmptyEntries);
                updatedSegments.AddRange(splits);
            }

            return updatedSegments.ToArray();
        }
    }
}
