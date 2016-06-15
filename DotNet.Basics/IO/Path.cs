using System;
using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public class Path
    {
        private readonly string[] _pathTokens;
        
        protected Path(string[] pathTokens, bool isFolder, PathDelimiter delimiter)
        {
            _pathTokens = pathTokens ?? new string[0];
            IsFolder = isFolder;
            Delimiter = delimiter;
        }

        public string[] PathTokens => _pathTokens;
        public bool IsFolder { get; private set; }
        public PathDelimiter Delimiter { get; set; }

        public string Name => PathTokens.Last();
        public string RawName => ToString(Delimiter);
        public string FullName => SystemIoPath.GetFullPath(RawName);
        public string NameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(Name);
        public string Extension => System.IO.Path.GetExtension(Name);

        public Path Parent
        {
            get
            {
                if (_pathTokens.Length <= 1)
                    return null;//no parent
                
                var allButLast = _pathTokens.Reverse().Skip(1).Reverse().ToArray();
                return new Path(allButLast, true, Delimiter);
            }
        }

        public Path Directory => IsFolder ? this : Parent;

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
            var path = string.Empty;
            foreach (var pathToken in PathTokens)
                path += $"{pathToken}{delimiter.ToChar()}";
            path = IsFolder ? path.EnsureSuffix(delimiter.ToChar()) : path.TrimEnd(delimiter.ToChar());
            return path;
        }
    }
}
