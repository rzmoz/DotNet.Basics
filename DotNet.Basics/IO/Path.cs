using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
            : this(protocol, path, DetectIsFolder(path))
        {
        }
        public Path(string path, bool isFolder)
            : this(null, path, isFolder)
        {
        }
        public Path(string protocol, string path, bool isFolder)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("path is not set");
            _pathTokens = new string[0];
            Protocol = protocol ?? string.Empty;
            Delimiter = DetectDelimiter(protocol, path);
            IsFolder = isFolder;
            Add(path);
        }

        public string Protocol { get; private set; }
        public string[] PathTokens => _pathTokens;
        public bool IsFolder { get; }
        public PathDelimiter Delimiter { get; set; }

        public string Name => PathTokens.Last();
        public string FullName => ToString(Delimiter);
        public string NameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(Name);
        public string Extension => System.IO.Path.GetExtension(Name);

        public Path Add(params string[] pathSegments)
        {
            foreach (var pathSegment in pathSegments)
                Add(pathSegment);
            return this;
        }

        public Path Add(string pathSegment)
        {
            if (string.IsNullOrWhiteSpace(pathSegment))
                throw new ArgumentException("pathSegment is not set");

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
            return this;
        }

        public override string ToString()
        {
            return ToString(Delimiter);
        }

        public string ToString(PathDelimiter delimiter)
        {
            var path = Protocol ?? string.Empty;
            foreach (var pathToken in PathTokens)
                path += $"{pathToken}{delimiter.ToChar()}";
            if (IsFolder == false)
                path = path.TrimEnd(delimiter.ToChar());
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
