using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Basics.IO
{
    public class TempWorkFile
    {
        public void Use(Action<FileInfo> task)
        {
            var tempFile = System.IO.Path.GetTempFileName().ToFile();
            try
            {
                tempFile.Directory.CreateIfNotExists();

                task.Invoke(tempFile);
            }
            finally
            {
                tempFile.DeleteIfExists();
            }
        }
    }
}
