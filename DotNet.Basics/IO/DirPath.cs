using System.Collections.Generic;
using System.Management.Automation;
using System.Security.AccessControl;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public class DirPath : PathInfo
    {
        public DirPath(string fullPath)
            : this(new[] { fullPath })
        { }

        public DirPath(IReadOnlyCollection<string> pathSegments) : base(pathSegments)
        { }

        public DirPath(IReadOnlyCollection<string> pathSegments, PathDelimiter delimiter)
            : base(pathSegments, true, delimiter)
        { }

        public void CleanIfExists()
        {
            if (IsFolder == false)
                throw new PathException($"Can't clean path because it's not a folder", this);
            try
            {
                PowerShellConsole.RemoveItem($"{FullName}\\*", force: true, recurse: true);
            }
            catch (ItemNotFoundException)
            { }
        }

        public void CreateIfNotExists()
        {
            if (this.Exists())
                return;

            if (IsFolder == false)
                throw new PathException($"Can't create path because it's not a folder {FullName}", this);

            try
            {
                System.IO.Directory.CreateDirectory(FullName);
            }
            catch (System.IO.IOException e)
            {
                DebugOut.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Returns a new Path where original and added paths are combined
        /// </summary>
        /// <param name="pathSegments"></param>
        /// <returns></returns>
        public new DirPath Add(params string[] pathSegments)
        {
            var combinedSegments = AddSegments(pathSegments);
            return new DirPath(combinedSegments, Delimiter);
        }

        public DirectorySecurity GetAccessControl()
        {
            return ToDirectoryInfo().GetAccessControl();
        }
        public void SetAccessControl(DirectorySecurity directorySecurity)
        {
            ToDirectoryInfo().SetAccessControl(directorySecurity);
        }
        public System.IO.DirectoryInfo ToDirectoryInfo()
        {
            return new System.IO.DirectoryInfo(RawName);
        }
    }
}
