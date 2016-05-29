using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Basics.IO
{
    public class TempWorkDir
    {
        public void Use(Action<DirectoryInfo> task)
        {
            var tempDir = System.IO.Path.GetTempPath().ToPath(Guid.NewGuid().ToString()).ToDir();
            try
            {
                tempDir.DeleteIfExists();
                tempDir.CreateIfNotExists();
                task.Invoke(tempDir);
            }
            finally
            {
                tempDir.ToDir().DeleteIfExists();
            }
        }
    }
}
