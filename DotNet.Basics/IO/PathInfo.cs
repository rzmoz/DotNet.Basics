using System;
using System.Linq;
using DotNet.Basics.Sys;
using Newtonsoft.Json;

namespace DotNet.Basics.IO
{
    public class PathInfo
    {
        private readonly string[] _segments;
        private readonly Func<string> _resolveFullName;

        protected PathInfo(string[] pathSegments, bool isFolder, PathDelimiter delimiter)
        {
            _segments = pathSegments.SplitToSegments();
            IsFolder = isFolder;
            Delimiter = delimiter;
            IsUri = false;

            try
            {
                var tryAsUri = RawName.Replace(PathInfoExtensions.Backslash, PathInfoExtensions.Slash);
                new Uri(tryAsUri);//will fail if not uri
                IsUri = RawName.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                        RawName.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                        RawName.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase) ||
                        RawName.StartsWith("sftp://", StringComparison.OrdinalIgnoreCase);
            }
            catch (UriFormatException)
            {
                IsUri = false;
            }

            if (IsUri)
            {
                Delimiter = PathDelimiter.Slash;
                _resolveFullName = () => RawName;
            }
            else
                _resolveFullName = () => SystemIoPath.GetFullPath(RawName);
        }

        public bool IsFolder { get; }
        [JsonIgnore]
        public PathDelimiter Delimiter { get; set; }

        [JsonIgnore]
        public string[] Segments => _segments;
        public bool IsUri { get; }
        public string Name => Segments.Last();
        public string RawName => ToString(Delimiter);
        public string FullName => _resolveFullName();
        public string NameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(Name);
        public string Extension => System.IO.Path.GetExtension(Name);
        [JsonIgnore]
        public DirPath Directory => IsFolder ? this.ToDir() : Parent;
        [JsonIgnore]
        public DirPath Parent
        {
            get
            {
                if (_segments.Length <= 1)
                {
                    var fullPath = FullName.ToPath();
                    return fullPath.Parent;
                }
                var allButLast = _segments.Reverse().Skip(1).Reverse().ToArray();
                return new DirPath(allButLast, Delimiter);
            }
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

        protected string[] AddSegments(params string[] pathSegments)
        {
            if (pathSegments == null)
                return Segments;

            var splitNewSegments = pathSegments.SplitToSegments();
            if (splitNewSegments.Length == 0)
                return Segments;

            var combinedSegments = Segments.Concat(pathSegments).ToArray();
            return combinedSegments;
        }

        public override string ToString()
        {
            return ToString(Delimiter);
        }

        public string ToString(PathDelimiter delimiter)
        {
            var path = string.Empty;
            foreach (var segment in Segments)
            {
                if (segment.IsProtocol())
                    path += segment;
                else
                    path += $"{segment}{delimiter.ToChar()}";
            }

            if (IsFolder == false)
                path = path.RemoveSuffix(delimiter.ToChar());
            return path;
        }

        protected bool Equals(PathInfo other)
        {
            return RawName == other.RawName;
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
            return RawName.GetHashCode();
        }
    }
}
