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
            : this(null, path, DetectIsFolder(path))
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
            Protocol = protocol;
            PathDelimiter delimiter;
            if (TryDetectDelimiter(protocol, out delimiter) == false)
                TryDetectDelimiter(path, out delimiter);
            Delimiter = delimiter;
            IsFolder = isFolder;
            Add(path);
        }

        public string Protocol { get; }
        public string[] PathTokens => _pathTokens;
        public bool IsFolder { get; }
        public PathDelimiter Delimiter { get; private set; }

        public string Name => PathTokens.Last();
        public string FullName => ToString(Delimiter);
        public string NameWithoutExtensions => System.IO.Path.GetFileNameWithoutExtension(Name);


        public void Add(string pathSegment)
        {
            Add(pathSegment, Delimiter);
        }
        public void Add(string pathSegment, PathDelimiter delimiter)
        {
            if (string.IsNullOrWhiteSpace(pathSegment))
                throw new ArgumentException("pathSegment is not set");

            Delimiter = delimiter;
            var updatedSegments = new List<string>(PathTokens);
            updatedSegments.AddRange(pathSegment.Split(new[] { _slashDelimiter, _backslashDelimiter }, StringSplitOptions.RemoveEmptyEntries));
            Interlocked.Exchange(ref _pathTokens, updatedSegments.ToArray());
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

        private bool TryDetectDelimiter(string path, out PathDelimiter delimiter)
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
