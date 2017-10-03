using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using DotNet.Basics.Tests.IO.TestSources;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.IO
{
    public class ExecutableInstallerTests
    {/*
        [Fact]
        public void InstallFromBytes_EnsureAppIsInstalled_AppIsInstalledInMultiThreadedEnvironment()
        {
            var appDir = "InstallFromBytes_EnsureAppIsInstalled_AppIsInstalledInMultiThreadedEnvironment".ToDir();
            appDir.DeleteIfExists();
            var fileName = "ReturnIntConsole.Exe";

            var installRange = Enumerable.Range(1, 50);
            Parallel.ForEach(installRange, val =>
            {
                var installer = new ExecutableInstaller(appDir, fileName);
                installer.AddFromBytes(fileName, IoTestResources.ReturnIntConsole);
                installer.Install();

                var fullName = appDir.ToFile(fileName).FullName;

                var returnCode = CommandPrompt.Run($"{fullName} {val}");
                returnCode.Should().Be(val);
            });
        }*/
    }
}
