using System.Linq;
using System.Threading.Tasks;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using DotNet.Basics.Tests.IO.TestSources;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.IO
{
    [TestFixture]
    public class ApplicationInstallerTests
    {
        [Test]
        public void InstallFromBytes_EnsureAppIsInstalled_AppIsInstalledInMultiThreadedEnvironment()
        {
            var appDir = TestContext.CurrentContext.TestDirectory.ToDir("InstallFromBytes_EnsureAppIsInstalled_AppIsInstalledInMultiThreadedEnvironment");
            appDir.DeleteIfExists();
            var fileName = "ReturnIntConsole.Exe";


            var installRange = Enumerable.Range(1, 50);
            Parallel.ForEach(installRange, val =>
            {
                var installer = new ApplicationInstaller(appDir, fileName);
                installer.AddFromBytes(fileName, IoTestResources.ReturnIntConsole);
                installer.Install();

                var fullName = appDir.ToFile(fileName).FullName;

                var returnCode = CommandPrompt.Run($"{fullName} {val}");
                returnCode.Should().Be(val);
            });
        }
    }
}
