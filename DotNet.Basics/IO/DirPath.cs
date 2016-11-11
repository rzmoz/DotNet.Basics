using System.Collections.Generic;
using System.IO;
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

        public DirPath(IReadOnlyCollection<string> pathSegments, char delimiter)
            : base(pathSegments, true, delimiter)
        { }

        public void CleanIfExists()
        {
            if (IsFolder == false)
                throw new IOException($"Can't clean path because path object is not a folder. IsFolder:{IsFolder}");
            try
            {
                PowerShellConsole.RemoveItem($"{FullName}\\*", force: true, recurse: true);
            }
            catch (ItemNotFoundException)
            { }
        }

        public void CreateIfNotExists()
        {
            if (Exists())
                return;

            if (IsFolder == false)
                throw new IOException($"Can't clean path because path object is not a folder. IsFolder:{IsFolder}");

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
