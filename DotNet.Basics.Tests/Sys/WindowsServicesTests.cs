using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Sys
{
    [TestFixture]
    public class WindowsServicesTests
    {
        [Test]
        public void Exists_Exists_ServiceExists()
        {
            var serviceName = "Spooler";

            var exists = WindowsServices.Exists(serviceName);

            exists.Should().BeTrue($"Service exists: {serviceName}");
        }

        [Test]
        public void Exists_Exists_ServiceDoesNotExist()
        {
            var serviceName = "Exists_Exists_ServiceDoesNotExist";

            var exists = WindowsServices.Exists(serviceName);

            exists.Should().BeFalse($"Service exists: {serviceName}");
        }

        [Test]
        public void Get_Status_StatusFromServicesIsRetrieved()
        {
            var serviceName = "Spooler";

            var isrunning = WindowsServices.IsRunning(serviceName);

            isrunning.Should().BeTrue(serviceName);
        }
    }
}
