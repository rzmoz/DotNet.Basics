using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using DotNet.Basics.Sys;
using DotNet.Basics.Tasks;

namespace DotNet.Basics.IO
{
    public class Path
    {
        private string[] _pathTokens;

        private const char _slashDelimiter = '/';
        private const char _backslashDelimiter = '\\';

        public Path(string path)
        {
            _pathTokens = new string[0];
            Protocol = string.Empty;
            Delimiter = DetectDelimiter(Protocol, path);
            Add(path);
        }
        public Path(string path, bool isFolder)
            : this(null, path, isFolder)
        {
        }
        public Path(string protocol, string path)
        {
            _pathTokens = new string[0];
            Protocol = protocol ?? string.Empty;
            Delimiter = DetectDelimiter(protocol, path);
            Add(path);
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
        public string FullName => ToString(Delimiter);
        public string NameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(Name);
        public string Extension => System.IO.Path.GetExtension(Name);

        public Path Add(params string[] pathSegments)
        {
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

        public override string ToString()
        {
            return ToString(Delimiter);
        }

        public bool Exists()
        {
            if (FullName.ToFile().Exists())
                return true;
            return FullName.ToDir().Exists();
        }

        public bool DeleteIfExists()
        {
            return DeleteIfExists(30.Seconds());
        }

        public bool DeleteIfExists(TimeSpan timeout)
        {
            return IsFolder ? FullName.ToDir().DeleteIfExists(timeout) : FullName.ToFile().DeleteIfExists(timeout);
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
