namespace DotNet.Basics.Sys
{
    public class FileType
    {
        public FileType(string nameWoExtensions, string extension)
        {
            NameWoExtension = nameWoExtensions ?? string.Empty;
            Extension = extension ?? string.Empty;
            if (!Extension.StartsWith("."))
                Extension = "." + Extension;
            Name = NameWoExtension + Extension;
            GetAllSearchPattern = "*" + Extension;
        }

        public string Name { get; }
        public string Extension { get; }
        public string NameWoExtension { get; }
        public string GetAllSearchPattern { get; }
    }
}
