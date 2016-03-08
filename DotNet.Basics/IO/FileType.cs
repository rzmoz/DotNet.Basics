namespace DotNet.Basics.IO
{
    public class FileType
    {
        public FileType(string name, string extension)
        {
            Name = name ?? string.Empty;
            Extension = extension ?? string.Empty;
            if (!Extension.StartsWith("."))
                Extension = "." + Extension;
            GetAllSearchPattern = "*" + Extension;
        }

        public string Name { get; }
        public string Extension { get; }

        public bool IsType(IoFile file)
        {
            if (file == null)
                return false;

            return IsType(file.Name);
        }

        public bool IsType(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return false;

            return filename.EndsWith(Extension, true, null);
        }

        public string GetAllSearchPattern { get; }
    }
}
