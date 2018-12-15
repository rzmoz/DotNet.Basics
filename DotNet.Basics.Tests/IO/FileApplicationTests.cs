using System.IO;
using System.Text;
using DotNet.Basics.IO;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.IO
{
    public class FileApplicationTests
    {
        [Fact]
        public void Install_FileWrite_FilesAreWrittenToDisk()
        {
            var fileName = "file.txt";
            var fileContent = "Hello World!";
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

            var fileApp = new FileApplication("TempoApp").WithStream(fileName, fileStream);
            fileApp.IsInstalled().Should().BeFalse();
            fileApp.InstallDir.ToFile(fileName).Exists().Should().BeFalse();
            //act
            fileApp.Install().Should().BeTrue();

            //assert
            fileApp.InstallDir.ToFile(fileName).Exists().Should().BeTrue();
            fileApp.UnInstall().Should().BeTrue();
            fileApp.InstallDir.ToFile(fileName).Exists().Should().BeFalse();
        }
        [Fact]
        public void Install_PostInstallActions_PostInstallActionIsExecuted()
        {
            var fileName = "file.txt";
            var newFileName = "new-file.txt";
            var fileContent = "Hello World!";
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

            var fileApp = new FileApplication("TempoApp2");
            fileApp.IsInstalled().Should().BeFalse();
            fileApp.InstallDir.ToFile(fileName).Exists().Should().BeFalse();
            fileApp.InstallDir.ToFile(newFileName).Exists().Should().BeFalse();
            //act
            fileApp = fileApp.WithStream(fileName, fileStream,
                file => { file.CopyTo(fileApp.InstallDir, newFileName); });
            fileApp.Install().Should().BeTrue();

            //assert
            fileApp.InstallDir.ToFile(newFileName).Exists().Should().BeTrue();
            fileApp.UnInstall().Should().BeTrue();
            fileApp.InstallDir.Exists().Should().BeFalse();
        }
    }
}
