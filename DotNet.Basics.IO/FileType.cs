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
        public string GetAllSearchPattern { get; }
    }
}
