using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using DotNet.Basics.Collections;
using DotNet.Basics.Sys;

namespace DotNet.Basics.IO
{
    public static class DirExtensions
    {
        

        public static void CopyTo(this IEnumerable<DirectoryInfo> sourceDirs, DirectoryInfo target, bool includeSubfolders = true)
        {
            sourceDirs.ForEach(dir => dir.CopyTo(target, includeSubfolders));
        }

        public static DirectoryInfo ToDir(this Path path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            return new DirectoryInfo(path.ToString(PathDelimiter.Backslash));
        }

        public static DirectoryInfo ToDir(this string dir, params string[] paths)
        {
            return new DirectoryInfo(new Path(dir).Add(paths).ToString(PathDelimiter.Backslash));
        }

        public static DirectoryInfo ToDir(this DirectoryInfo dir, params string[] paths)
        {
            return dir.FullName.ToDir(paths);
        }

        public static DirectoryInfo CreateSubdir(this DirectoryInfo dir, string path)
        {
            var subDir = dir.ToDir(path);
            subDir.CreateIfNotExists();
            return subDir;
        }

        public static void ConsolidateIdenticalSubfolders(this DirectoryInfo dir, int lookDepth = int.MaxValue)
        {
            if (dir.Exists() == false)
                throw new IOException(dir.FullName);

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
                Robocopy.Move(source.FullName, target.FullName);
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
                    Robocopy.Move(source.FullName, target.FullName);
                }
            }

            //we delete the folder if it's empty if everything was moved - otherwise, we don't 
            if (dir.IsEmpty() && !subDirIsIdenticalToParentDir)
                dir.DeleteIfExists();
        }

        private static bool ParentHasIdenticalName(this DirectoryInfo dir)
        {
            if (dir.Exists() == false)
                return false;
            if (dir.Parent == null)
                return false;
            return dir.Name.Equals(dir.Parent.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public static void CopyTo(this DirectoryInfo source, DirectoryInfo target, bool includeSubfolders = false)
        {
            if (source.Exists() == false)
            {
                Debug.WriteLine("Source '{0}' not found. Aborting", source.FullName);
                return;
            }

            if (source.FullName.Equals(target.FullName, StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine("Source and Target are the same '{0}'. Aborting", source.FullName);
                return;
            }

            try
            {
                PowerShellConsole.CopyItem(source.FullName, target.FullName, force: true, recurse: includeSubfolders);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Fast copy failed - falling back to use robocopy\r\n{0}", e);
                Robocopy.CopyDir(source.FullName, target.FullName, includeSubFolders: includeSubfolders);
            }
        }

        public static void CleanIfExists(this DirectoryInfo dir)
        {
            if (dir == null) throw new ArgumentNullException(nameof(dir));
            PowerShellConsole.RemoveItem($"{dir.FullName}\\*", force: true, recurse: true);
        }

        public static void CreateIfNotExists(this DirectoryInfo dir)
        {
            if (dir.Exists())
                return;
            PowerShellConsole.NewItem(dir.FullName, "Directory", false);
            Debug.WriteLine($"Created: {dir.FullName}");
        }

        public static void GrantAccess(this DirectoryInfo dir, string username, FileSystemRights fileSystemRights = FileSystemRights.FullControl)
        {
            if (dir.Exists() == false)
                return;
            Debug.WriteLine("Giving {0} user write access to: {1}", username, dir.FullName);

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

        public static bool IsEmpty(this DirectoryInfo dir)
        {
            if (dir == null) throw new ArgumentNullException(nameof(dir));
            dir.Refresh();
            if (dir.Exists())
                return dir.EnumerateFiles().Any() == false && dir.EnumerateDirectories().Any() == false;
            return true;
        }


        private static void CanonicalizeDacl(NativeObjectSecurity objectSecurity)
        {
            if (objectSecurity == null) { throw new ArgumentNullException(nameof(objectSecurity)); }
            if (objectSecurity.AreAccessRulesCanonical) { return; }

            // A canonical ACL must have ACES sorted according to the following order:
            //   1. Access-denied on the object
            //   2. Access-denied on a child or property
            //   3. Access-allowed on the object
            //   4. Access-allowed on a child or property
            //   5. All inherited ACEs 
            var descriptor = new RawSecurityDescriptor(objectSecurity.GetSecurityDescriptorSddlForm(AccessControlSections.Access));

            var implicitDenyDacl = new List<CommonAce>();
            var implicitDenyObjectDacl = new List<CommonAce>();
            var inheritedDacl = new List<CommonAce>();
            var implicitAllowDacl = new List<CommonAce>();
            var implicitAllowObjectDacl = new List<CommonAce>();

            foreach (CommonAce ace in descriptor.DiscretionaryAcl)
            {
                if ((ace.AceFlags & AceFlags.Inherited) == AceFlags.Inherited) { inheritedDacl.Add(ace); }
                else
                {
                    switch (ace.AceType)
                    {
                        case AceType.AccessAllowed:
                            implicitAllowDacl.Add(ace);
                            break;

                        case AceType.AccessDenied:
                            implicitDenyDacl.Add(ace);
                            break;

                        case AceType.AccessAllowedObject:
                            implicitAllowObjectDacl.Add(ace);
                            break;

                        case AceType.AccessDeniedObject:
                            implicitDenyObjectDacl.Add(ace);
                            break;
                    }
                }
            }

            Int32 aceIndex = 0;
            RawAcl newDacl = new RawAcl(descriptor.DiscretionaryAcl.Revision, descriptor.DiscretionaryAcl.Count);
            implicitDenyDacl.ForEach(x => newDacl.InsertAce(aceIndex++, x));
            implicitDenyObjectDacl.ForEach(x => newDacl.InsertAce(aceIndex++, x));
            implicitAllowDacl.ForEach(x => newDacl.InsertAce(aceIndex++, x));
            implicitAllowObjectDacl.ForEach(x => newDacl.InsertAce(aceIndex++, x));
            inheritedDacl.ForEach(x => newDacl.InsertAce(aceIndex++, x));

            if (aceIndex != descriptor.DiscretionaryAcl.Count)
            {
                //"The DACL cannot be canonicalized since it would potentially result in a loss of information";
                return;
            }

            descriptor.DiscretionaryAcl = newDacl;
            objectSecurity.SetSecurityDescriptorSddlForm(descriptor.GetSddlForm(AccessControlSections.Access), AccessControlSections.Access);
        }
    }
}
