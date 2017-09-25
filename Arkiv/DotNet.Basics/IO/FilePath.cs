using System;
using System.Collections.Generic;
using System.IO;

namespace DotNet.Basics.IO
{
    public class FilePath : PathInfo
    {
        public FilePath(string fullPath)
            : this(new[] { fullPath })
        {
        }

        public FilePath(IReadOnlyCollection<string> pathSegments) : base(pathSegments)
        {
        }

        public FilePath(IReadOnlyCollection<string> pathSegments, char delimiter)
            : base(pathSegments, false, delimiter)
        {
        }

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

        public string ReadAllText(bool throwIfNotExists = true)
        {
            try
            {
                return File.ReadAllText(FullName);
            }
            catch (DirectoryNotFoundException)
            {
                if (throwIfNotExists)
                    throw;
            }
            catch (FileNotFoundException)
            {
                if (throwIfNotExists)
                    throw;
            }
            return null;
        }
    }
}
