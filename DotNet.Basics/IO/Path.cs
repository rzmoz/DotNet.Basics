using System;
using System.Collections.Generic;
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
            //we assume most path are slash - only io paths are using backslash
            Delimiter = DetectDelimiter(path);
            IsFolder = isFolder;
            Add(path);
        }

        public string Protocol { get; }
        public string[] PathTokens => _pathTokens;
        public bool IsFolder { get; }
        public PathDelimiter Delimiter { get; }

        public string Name => System.IO.Path.GetFileName(FullName);
        public string FullName => ToString(Delimiter);
        public string NameWithoutExtensions => System.IO.Path.GetFileNameWithoutExtension(FullName);


        public void Add(string pathSegment)
        {
            if (string.IsNullOrWhiteSpace(pathSegment))
                throw new ArgumentException("pathSegment is not set");

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
                path += $"{pathToken}{Delimiter.ToChar()}";
            if (IsFolder == false)
                path = path.TrimEnd(Delimiter.ToChar());
            return path;
        }

        private static bool DetectIsFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            return path.EndsWith(_slashDelimiter.ToString()) || path.EndsWith(_backslashDelimiter.ToString());
        }

        private PathDelimiter DetectDelimiter(string path)
        {
            if (path == null || path.IndexOf(_slashDelimiter) < 0)
                return PathDelimiter.Backslash;
            return PathDelimiter.Slash;
        }
    }
}
