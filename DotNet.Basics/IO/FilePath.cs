using System.IO;
using System.Linq;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public class FilePath : PathInfo
    {
        public FilePath(string fullPath)
            : this(new[] { fullPath })
        { }

        public FilePath(string[] pathSegments)
            : this(pathSegments, PathDelimiter.Backslash)
        { }

        public FilePath(string[] pathSegments, PathDelimiter delimiter)
            : base(pathSegments, false, delimiter)
        { }

        /// <summary>
        /// Returns a new Path where original and added paths are combined
        /// </summary>
        /// <param name="pathSegments"></param>
        /// <returns></returns>
        public new FilePath Add(params string[] pathSegments)
        {
            var combinedSegments = AddSegments(pathSegments);
            return new FilePath(combinedSegments, Delimiter);
        }

        public bool IsFileType(FileType fileType)
        {
            if (fileType == null)
                return false;
            return Name.EndsWith(fileType.Extension, true, null);
        }

        public string ReadAllText()
        {
            return File.ReadAllText(FullName);
        }
    }
}
