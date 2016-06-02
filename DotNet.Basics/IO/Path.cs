using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public class Path
    {
        private string[] _pathTokens;

        private const char _slashDelimiter = '/';
        private const char _backslashDelimiter = '\\';

        public Path(string path)
            : this(null, path)
        {
        }

        public Path(string protocol, string path)
        {
            _pathTokens = new string[0];
            Protocol = protocol ?? string.Empty;
            Delimiter = DetectDelimiter(protocol, path);
            Add(path);
        }
        public Path(string path, bool isFolder)
            : this(null, path, isFolder)
        {
        }
        public Path(string protocol, string path, bool isFolder)
        {
            _pathTokens = new string[0];
            Protocol = protocol ?? string.Empty;
            Delimiter = DetectDelimiter(protocol, path);
            Add(path, isFolder);
        }

        public string Protocol { get; private set; }
        public string[] PathTokens => _pathTokens;
        public bool IsFolder { get; set; }
        public PathDelimiter Delimiter { get; set; }

        public string Name => PathTokens.Last();
        public string RelativeName => ToString(Delimiter);
        public string FullName => Delimon.Win32.IO.Path.GetFullPath(RelativeName);
        public string NameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(Name);
        public string Extension => System.IO.Path.GetExtension(Name);

        public Path Parent
        {
            get
            {
                if (_pathTokens.Length <= 1)
                    return null;//no parent

                var parentPath = new Path(Protocol);
                var allButLast = _pathTokens.Reverse().Skip(1).Reverse().ToArray();
                parentPath.Add(allButLast);
                parentPath.IsFolder = true;//parent is always folder
                return parentPath;
            }
        }

        public Path Directory => IsFolder ? this : Parent;

        public Path Add(params string[] pathSegments)
        {
            if (pathSegments.Length == 0)
                return this;

            var isFolder = DetectIsFolder(pathSegments.Last());
            Add(isFolder, pathSegments);
            return this;
        }
        public Path Add(bool isFolder, params string[] pathSegments)
        {
            foreach (var pathSegment in pathSegments)
                Add(pathSegment, isFolder);
            return this;
        }

        public Path Add(string pathSegment)
        {
            var isFolder = DetectIsFolder(pathSegment);
            return Add(pathSegment, isFolder);
        }

        public Path Add(string pathSegment, bool isFolder)
        {
            if (string.IsNullOrWhiteSpace(pathSegment))
                return this;

            var updatedSegments = new List<string>(PathTokens);
            updatedSegments.AddRange(pathSegment.Split(new[] { _slashDelimiter, _backslashDelimiter }, StringSplitOptions.RemoveEmptyEntries));

            //leading delimiter detected
            bool shouldUpdateProtocol = true;
            shouldUpdateProtocol = shouldUpdateProtocol && string.IsNullOrWhiteSpace(Protocol);//protocol is already set
            shouldUpdateProtocol = shouldUpdateProtocol && PathTokens.Length == 0;//this is not the initial path added
            shouldUpdateProtocol = shouldUpdateProtocol && (pathSegment.StartsWith(_slashDelimiter.ToString()) || pathSegment.StartsWith(_backslashDelimiter.ToString()));

            if (shouldUpdateProtocol)
            {
                //leading delimiters goes to protocol
                var leadingDelimiter = pathSegment[0];
                foreach (char c in pathSegment)
                {
                    if (c == leadingDelimiter)
                        Protocol += c.ToString();
                    else
                        break;
                }
            }
            Interlocked.Exchange(ref _pathTokens, updatedSegments.ToArray());
            IsFolder = isFolder;
            return this;
        }

        private sealed class ProtocolEqualityComparer : IEqualityComparer<Path>
        {
            public bool Equals(Path x, Path y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.FullName, y.FullName);
            }

            public int GetHashCode(Path obj)
            {
                return obj.FullName?.GetHashCode() ?? 0;
            }
        }

        public static IEqualityComparer<Path> ProtocolComparer { get; } = new ProtocolEqualityComparer();

        public override string ToString()
        {
            return ToString(Delimiter);
        }

        public string ToString(PathDelimiter delimiter)
        {
            var path = Protocol ?? string.Empty;
            foreach (var pathToken in PathTokens)
                path += $"{pathToken}{delimiter.ToChar()}";
            path = IsFolder ? path.EnsureSuffix(delimiter.ToChar()) : path.TrimEnd(delimiter.ToChar());
            return path;
        }

        private bool DetectIsFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            if (path.Trim(_slashDelimiter) == "" || path.Trim(_backslashDelimiter) == "")
                return IsFolder;//if it's only a delimiter, then we return existing value

            return path.EndsWith(_slashDelimiter.ToString()) || path.EndsWith(_backslashDelimiter.ToString());
        }

        private static PathDelimiter DetectDelimiter(string protocol, string path)
        {
            PathDelimiter delimiter;
            if (TryDetectDelimiter(protocol, out delimiter) == false)
                TryDetectDelimiter(path, out delimiter);
            return delimiter;
        }
        private static bool TryDetectDelimiter(string path, out PathDelimiter delimiter)
        {
            if (path == null)
            {
                delimiter = PathDelimiter.Backslash;
                return false;
            }

            var slashIndex = path.IndexOf(_slashDelimiter);
            var backSlashIndex = path.IndexOf(_backslashDelimiter);

            if (slashIndex < 0)
            {
                delimiter = PathDelimiter.Backslash;
                return true;
            }

            if (backSlashIndex < 0)
            {
                delimiter = PathDelimiter.Slash;
                return true;
            }

            //first occurence decides
            delimiter = slashIndex < backSlashIndex ? PathDelimiter.Slash : PathDelimiter.Backslash;
            return true;
        }
    }
}
