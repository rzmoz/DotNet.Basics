using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;

namespace DotNet.Basics.IO
{
    public static class DirExtensions
    {
        public static void CopyTo(this IEnumerable<IoDir> sourceDirs, IoDir target)
        {
            foreach (var dir in sourceDirs)
            {
                var targetFolder = new IoDir(target, dir.Name);
                dir.CopyTo(targetFolder, DirCopyOptions.IncludeSubDirectories);
            }
        }

        public static IoDir ToDir(this string directory, params string[] paths)
        {
            return new IoDir(directory, paths);
        }
        public static IoDir ToDir(this DirectoryInfo directory, params string[] paths)
        {
            return new IoDir(directory, paths);
        }
        public static IoDir ToDir(this IoDir directory, params string[] paths)
        {
            return new IoDir(directory, paths);
        }

        public static IoDir CreateSubdir(this IoDir directory, string path)
        {
            var subDir = directory.ToDir(path);
            subDir.CreateIfNotExists();
            return subDir;
        }

        public static void CreateIfNotExists(this IoDir dir, DirCreateOptions dirCreateOptions = DirCreateOptions.DontCleanIfExists)
        {
            if (dir.Exists())
            {
                if (dirCreateOptions == DirCreateOptions.CleanIfExists)
                    dir.CleanIfExists();
                return;
            }

            Directory.CreateDirectory(dir.FullName);
            Debug.WriteLine($"Created: {dir.FullName}");
        }

        public static void GrantAccess(this IoDir dir, string username, FileSystemRights fileSystemRights = FileSystemRights.FullControl)
        {
            if (Directory.Exists(dir.FullName) == false)
                return;
            Debug.WriteLine("Giving {0} user write access to: {1}", username, dir.FullName);

            DirectorySecurity directorySecurity = ((DirectoryInfo)dir.FileSystemInfo).GetAccessControl();
            CanonicalizeDacl(directorySecurity);

            directorySecurity.AddAccessRule(new FileSystemAccessRule(
                                    username,
                                    fileSystemRights,
                                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                                    PropagationFlags.None,
                                    AccessControlType.Allow));

            ((DirectoryInfo)dir.FileSystemInfo).SetAccessControl(directorySecurity);
        }

        public static bool IsEmpty(this IoDir dir)
        {
            if (dir == null) throw new ArgumentNullException(nameof(dir));
            dir.Refresh();

            return dir.EnumerateFiles().Any() == false && dir.EnumerateDirectories().Any() == false;
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
