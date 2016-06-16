using System;
using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public class Path
    {
        private readonly string[] _segments;

        protected Path(string[] pathSegments, bool isFolder, PathDelimiter delimiter)
        {
            _segments = pathSegments.SplitSegments();
            IsFolder = isFolder;
            Delimiter = delimiter;
        }

        public string[] Segments => _segments;
        public bool IsFolder { get; }
        public PathDelimiter Delimiter { get; set; }

        public string Name => Segments.Last();
        public string RawName => ToString(Delimiter);
        public string FullName => SystemIoPath.GetFullPath(RawName);
        public string NameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(Name);
        public string Extension => System.IO.Path.GetExtension(Name);
        public Path Directory => IsFolder ? this : Parent;

        public Path Parent
        {
            get
            {
                if (_segments.Length <= 1)
                    return null;//no parent

                var allButLast = _segments.Reverse().Skip(1).Reverse().ToArray();
                return new Path(allButLast, true, Delimiter);
            }
        }
        
        public override string ToString()
        {
            return ToString(Delimiter);
        }

        public string ToString(PathDelimiter delimiter)
        {
            var path = string.Empty;
            foreach (var pathToken in Segments)
                path += $"{pathToken}{delimiter.ToChar()}";

            if (IsFolder == false)
                path = path.RemoveSuffix(delimiter.ToChar());
            return path;
        }
    }
}
