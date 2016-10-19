using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Security.AccessControl;
using System.Threading.Tasks;
using DotNet.Basics.Collections;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public static class DirExtensions
    {
        public static DirPath ToDir(this PathInfo path, params string[] pathSegments)
        {
            return new DirPath(path.Segments, path.Delimiter).Add(pathSegments);
        }

        public static DirPath ToDir(this string dir, params string[] pathSegments)
        {
            return new DirPath(dir).Add(pathSegments);
        }

        public static void CopyTo(this IEnumerable<DirPath> sourceDirs, DirPath target, bool includeSubfolders = true)
        {
            sourceDirs.ForEach(dir => dir.CopyTo(target, includeSubfolders));
        }

        public static DirPath CreateSubDir(this DirPath dir, string path)
        {
            var subDir = dir.Add(path);
            subDir.CreateIfNotExists();
            return subDir;
        }

        public static void ConsolidateIdenticalSubfolders(this DirPath dir, int lookDepth = int.MaxValue)
        {
            if (dir.Exists() == false)
                throw new PathException(dir);

            //depth first recursive
            if (lookDepth > 0)//we only look to a certain depth
                foreach (var subDir in dir.GetDirectories())
                {
                    subDir.ConsolidateIdenticalSubfolders(lookDepth - 1);//decrement look depth as a stop criteria
                }

            //if folder was deleted during consolidation
            if (dir.Exists() == false)
                return;

            //we move this dir up as long up the hieararchy as long as the folder names are identical
            if (dir.ParentHasIdenticalName() == false)
                return;

            bool subDirIsIdenticalToParentDir = false;

            foreach (var source in dir.GetDirectories())
            {
                var target = dir.Parent.ToDir(source.Name);
                if (target.FullName.Equals(dir.FullName, StringComparison.InvariantCultureIgnoreCase))
                    subDirIsIdenticalToParentDir = true;
                Robocopy.MoveFolder(source.FullName, target.FullName, null, true);
            }

            if (subDirIsIdenticalToParentDir == false)
            {
                var source = dir;
                var target = dir.Parent;
                try
                {
                    PowerShellConsole.MoveItem($"{source.FullName}\\*.*", target.FullName, true);
                }
                catch (Exception)
                {
                    Robocopy.MoveFolder(source.FullName, target.FullName);
                }
            }

            //we delete the folder if it's empty if everything was moved - otherwise, we don't 
            if (dir.IsEmpty() && !subDirIsIdenticalToParentDir)
                dir.DeleteIfExists();
        }

        private static bool ParentHasIdenticalName(this DirPath dir)
        {
            if (dir.Exists() == false)
                return false;
            if (dir.Parent == null)
                return false;
            return dir.Name.Equals(dir.Parent.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public static void CopyTo(this DirPath source, DirPath target, bool includeSubfolders = false)
        {
            if (source.Exists() == false)
            {
                DebugOut.WriteLine($"Source '{source.FullName}' not found. Aborting");
                return;
            }

            if (source.FullName.Equals(target.FullName, StringComparison.OrdinalIgnoreCase))
            {
                DebugOut.WriteLine($"Source and Target are the same '{source.FullName}'. Aborting");
                return;
            }

            try
            {
                target.CreateIfNotExists();

                //depth first to find out quickly if we have long path exceptions - we want to fail early then
                if (includeSubfolders)
                {
                    Parallel.ForEach(source.GetDirectories(), dir =>
                    {
                        var nextTargetSubDir = target.ToDir(dir.Name);
                        nextTargetSubDir.CreateIfNotExists();
                        dir.CopyTo(nextTargetSubDir, includeSubfolders);
                    });
                }

                Parallel.ForEach(source.GetFiles(), file =>
                {
                    file.CopyTo(target, overwrite: true);
                });
            }
            catch (Exception e)
            {
                DebugOut.WriteLine($"Fast copy failed - falling back to use robocopy\r\n{e}");
                Robocopy.CopyDir(source.FullName, target.FullName, includeSubFolders: includeSubfolders);
            }
        }

        public static void CleanIfExists(this DirPath dir)
        {
            if (dir == null)
                return;
            try
            {
                PowerShellConsole.RemoveItem($"{dir.FullName}\\*", force: true, recurse: true);
            }
            catch (ItemNotFoundException)
            {
                //kill exceptions if not found
            }
        }

        public static bool IsEmpty(this DirPath dir)
        {
            if (dir == null) throw new ArgumentNullException(nameof(dir));
            if (dir.Exists())
                return dir.EnumeratePaths().Any() == false;
            return true;
        }

        public static void GrantAccess(this DirPath dir, string username, FileSystemRights fileSystemRights = FileSystemRights.Read)
        {
            if (dir.Exists() == false)
                return;

            DirectorySecurity directorySecurity = dir.GetAccessControl();
            CanonicalizeDacl(directorySecurity);

            directorySecurity.AddAccessRule(new FileSystemAccessRule(
                                    username,
                                    fileSystemRights,
                                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                                    PropagationFlags.None,
                                    AccessControlType.Allow));

            dir.SetAccessControl(directorySecurity);
        }

        private static void CanonicalizeDacl(NativeObjectSecurity objectSecurity)
        {
            if (objectSecurity == null) { throw new ArgumentNullException(nameof(objectSecurity)); }
            if (objectSecurity.AreAccessRulesCanonical) { return; }


            var descriptor = new RawSecurityDescriptor(objectSecurity.GetSecurityDescriptorSddlForm(AccessControlSections.Access));
            var result = CanonicalizeDacl(descriptor);

            //"The DACL cannot be canonicalized since it would potentially result in a loss of information";
            if (result.AceIndex != descriptor.DiscretionaryAcl.Count)
                return;

            descriptor.DiscretionaryAcl = result.RawAcl;
            objectSecurity.SetSecurityDescriptorSddlForm(descriptor.GetSddlForm(AccessControlSections.Access), AccessControlSections.Access);
        }

        private class CanonicalizeResult
        {
            public CanonicalizeResult(int aceIndex, RawAcl rawAcl)
            {
                AceIndex = aceIndex;
                RawAcl = rawAcl;
            }

            public int AceIndex { get; }
            public RawAcl RawAcl { get; }
        }

        private static CanonicalizeResult CanonicalizeDacl(RawSecurityDescriptor descriptor)
        {
            var newDacl = new RawAcl(descriptor.DiscretionaryAcl.Revision, descriptor.DiscretionaryAcl.Count);
            var aceIndex = 0;
            var commonAces = descriptor.DiscretionaryAcl.Cast<CommonAce>().ToList();

            // A canonical ACL must have ACES sorted according to the following order:
            //   1. Access-denied on the object
            Add(newDacl, commonAces.Where(ace => ace.AceType == AceType.AccessDenied), ref aceIndex);
            //   2. Access-denied on a child or property
            Add(newDacl, commonAces.Where(ace => ace.AceType == AceType.AccessAllowedObject), ref aceIndex);
            //   3. Access-allowed on the object
            Add(newDacl, commonAces.Where(ace => ace.AceType == AceType.AccessAllowed), ref aceIndex);
            //   4. Access-allowed on a child or property
            Add(newDacl, commonAces.Where(ace => ace.AceType == AceType.AccessAllowedObject), ref aceIndex);
            //   5. All inherited ACEs 
            Add(newDacl, commonAces.Where(ace => (ace.AceFlags & AceFlags.Inherited) == AceFlags.Inherited), ref aceIndex);

            return new CanonicalizeResult(aceIndex, newDacl);
        }

        private static void Add(RawAcl acl, IEnumerable<CommonAce> aces, ref int aceIndex)
        {
            foreach (var ace in aces)
                acl.InsertAce(aceIndex++, ace);
        }
    }
}
