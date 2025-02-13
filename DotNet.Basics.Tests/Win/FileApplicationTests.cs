using DotNet.Basics.Win;
using System.IO;
using System.Text;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit.Abstractions;
using Xunit;

namespace DotNet.Basics.Tests.Win
{
    public class FileApplicationTests(ITestOutputHelper output, string testPathPrefix = null)
        : TestWithHelpers(output, testPathPrefix)
    {
        [Fact]
        public void Install_FileWrite_FilesAreWrittenToDisk()
        {
            WithTestRoot(dir =>
            {
                var fileName = "file.txt";
                var fileContent = "Hello World!";
                var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

                var fileApp = new FileApplication(dir.Add("TempoApp")).WithStream(fileName, fileStream);
                fileApp.IsInstalled().Should().BeFalse();
                fileApp.InstallDir.ToFile(fileName).Exists().Should().BeFalse();
                //act
                fileApp.Install().Should().BeTrue();

                //assert
                fileApp.InstallDir.ToFile(fileName).Exists().Should().BeTrue();
                fileApp.UnInstall().Should().BeTrue();
                fileApp.InstallDir.ToFile(fileName).Exists().Should().BeFalse();
            });

        }
        [Fact]
        public void Install_PostInstallActions_PostInstallActionIsExecuted()
        {
            WithTestRoot(dir =>
            {
                var fileName = "file.txt";
                var newFileName = "new-file.txt";
                var fileContent = "Hello World!";
                var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

                var fileApp = new FileApplication(dir.Add("TempoApp2"));
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
            });
        }
    }
}
