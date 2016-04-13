using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DotNet.Basics.IO
{
    public class IoFile : IoPath
    {
        public IoFile(string root, params string[] paths)
            : base(root, paths)
        { }

        public IoFile(IoDir root, string fileName)
            : base(root, ToArray(fileName))
        { }

        public IoFile(FileSystemInfo root, params string[] paths)
            : base(root, paths)
        { }

        public IoDir Directory => ((FileInfo)FileSystemInfo).Directory.ToDir();

        public async Task<string> ReadAllTextAsync()
        {
            using (var reader = File.OpenText(FullName))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public void CopyTo(IoDir targetDir, FileCopyOptions fileCopyOptions = FileCopyOptions.AbortIfExists)
        {
            CopyTo(this, targetDir, fileCopyOptions);
        }

        public void CopyTo(IoFile newFileName, FileCopyOptions fileCopyOptions = FileCopyOptions.AbortIfExists)
        {
            CopyTo(newFileName, newFileName.Directory, fileCopyOptions);
        }

        public void CopyTo(IoFile newFileName, IoDir baseDir, FileCopyOptions fileCopyOptions = FileCopyOptions.AbortIfExists)
        {
            var targetFile = baseDir.ToFile(newFileName);
            targetFile.Directory.CreateIfNotExists();
            File.Copy(FullName, targetFile.FullName, fileCopyOptions == FileCopyOptions.OverwriteIfExists);
            Debug.WriteLine("{0} copied to: {1}", FullName, targetFile.FullName);
        }

        public void MoveTo(IoDir targetDir, bool overwrite = false)
        {
            MoveTo(new IoFile(targetDir, Name), overwrite);
        }
        public void MoveTo(IoFile targetFile, bool overwrite = false)
        {
            if (FullName == targetFile.FullName)
            {
                Debug.WriteLine("MoveTo skipped. Source and target are the same: {0}", FullName);
                return;
            }

            if (!Exists())
            {
                Debug.WriteLine("MoveTo skipped. Source not found: {0}", FullName);
                return;
            }

            if (overwrite == false && targetFile.Exists())
            {
                Debug.WriteLine("MoveTo skipped. Target already exists and overwrite is false: {0}", targetFile.FullName);
                return;
            }

            targetFile.DeleteIfExists();
            File.Move(FullName, targetFile.FullName);
        }

        private static string[] ToArray(string str)
        {
            return new[] { str };
        }
    }
}
