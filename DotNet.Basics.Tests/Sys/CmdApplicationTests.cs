using System.IO;
using System.Text;
using DotNet.Basics.Sys;
using DotNet.Basics.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class CmdApplicationTests
    {
        [Fact]
        public void Install_FileWrite_FilesAreWrittenToDisl()
        {
            var fileName = "file.txt";
            var fileContent = "Hello World!";
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

            var cmdApp = new CmdApplication("TempoApp", fileName, fileStream);
            cmdApp.IsInstalled().Should().BeFalse();
            cmdApp.EntryFile.Exists().Should().BeFalse();
            //act
            cmdApp.Install().Should().BeTrue();

            //assert
            cmdApp.EntryFile.Exists().Should().BeTrue();
            cmdApp.UnInstall().Should().BeTrue();
            cmdApp.EntryFile.Exists().Should().BeFalse();
        }
    }
}
