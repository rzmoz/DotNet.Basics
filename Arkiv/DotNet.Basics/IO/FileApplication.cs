namespace DotNet.Basics.IO
{
    public class FileApplication
    {
        public FileApplication(DirPath appDataFolder, string name)
        {
            AppDatafolder = appDataFolder;
            Name = name;
            Root = appDataFolder.ToDir(name);
        }

        public string Name { get; }
        public DirPath AppDatafolder { get; }
        public DirPath Root { get; }

        public void Install(params byte[] files)
        { }

        private void Extract(string fileName, byte[] bytes)
        {
            var target = Root.ToFile(fileName).FullName;
            using (var fsDst = new System.IO.FileStream(target, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write))
                fsDst.Write(bytes, 0, bytes.Length);
        }
    }
}
