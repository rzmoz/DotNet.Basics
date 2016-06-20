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
        public DirPath Directory => IsFolder ? this.ToDir() : Parent;

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
        public Path Add(params string[] pathSegments)
        {
            var combinedSegments = AddSegments(pathSegments);
            return new Path(combinedSegments, IsFolder, Delimiter);
        }

        protected string[] AddSegments(params string[] pathSegments)
        {
            if (pathSegments == null)
                return Segments;

            var splitNewSegments = pathSegments.SplitSegments();
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
            foreach (var pathToken in Segments)
                path += $"{pathToken}{delimiter.ToChar()}";

            if (IsFolder == false)
                path = path.RemoveSuffix(delimiter.ToChar());
            return path;
        }
    }
}
