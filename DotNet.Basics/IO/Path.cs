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

        public Path(string path, DetectOptions detectOptions = DetectOptions.AutoDetect)
            : this(null, path, detectOptions)
        {
        }
        public Path(string protocol, string path, DetectOptions detectOptions = DetectOptions.AutoDetect)
        {
            _pathTokens = new string[0];
            Protocol = protocol ?? string.Empty;
            Delimiter = DetectDelimiter(protocol, path);
            Add(detectOptions, path);
        }

        public string Protocol { get; private set; }
        public string[] PathTokens => _pathTokens;
        public bool IsFolder { get; private set; }
        public PathDelimiter Delimiter { get; set; }

        public string Name => PathTokens.Last();
        public string RelativeName => ToString(Delimiter);
        public string FullName => SystemIoPath.GetFullPath(RelativeName);
        public string NameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(Name);
        public string Extension => System.IO.Path.GetExtension(Name);

        public Path Parent
        {
            get
            {
                if (_pathTokens.Length <= 1)
                    return null;//no parent

                var parentPath = new Path(Protocol, DetectOptions.SetToDir);
                var allButLast = _pathTokens.Reverse().Skip(1).Reverse().ToArray();
                parentPath.Add(DetectOptions.SetToDir, allButLast);
                return parentPath;
            }
        }

        public Path Directory => IsFolder ? this : Parent;

        public Path Add(params string[] pathSegments)
        {
            return Add(DetectOptions.AutoDetect, pathSegments);
        }
        public Path Add(DetectOptions detectOptions = DetectOptions.AutoDetect, params string[] pathSegments)
        {
            if (pathSegments == null || pathSegments.Length == 0)
                return this;

            foreach (var pathSegment in pathSegments)
                Add(pathSegment, detectOptions);
            return this;
        }

        public Path Add(string pathSegment, DetectOptions detectOptions = DetectOptions.AutoDetect)
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
            switch (detectOptions)
            {
                case DetectOptions.SetToDir:
                    IsFolder = true;
                    break;
                case DetectOptions.SetToFile:
                    IsFolder = false;
                    break;
                case DetectOptions.AutoDetect:
                    IsFolder = DetectIsFolder(pathSegment);
                    break;
                case DetectOptions.KeepExisting:
                    break;
            }
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

        private static bool DetectIsFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

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
