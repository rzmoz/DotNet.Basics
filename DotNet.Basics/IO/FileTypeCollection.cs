using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DotNet.Basics.IO
{
    public class FileTypeCollection : IEnumerable<string>
    {
        protected ConcurrentBag<string> KnownFileTypes { get; }

        public FileTypeCollection()
        {
            KnownFileTypes = new ConcurrentBag<string>();
        }

        public void Add(FileType fileType)
        {
            var extension = fileType.Extension.ToLower();
            if (extension.StartsWith(".") == false)
                extension = "." + extension;

            KnownFileTypes.Add(extension);
        }

        public bool IsRegisteredFileType(FilePath file)
        {
            return KnownFileTypes.Any(fileType => file.Name.EndsWith(fileType, true, CultureInfo.InvariantCulture));
        }

        public IEnumerator<string> GetEnumerator()
        {
            return KnownFileTypes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
