using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys
{
    public class WindowsServicesTests
    {
        [Fact]
        public void Exists_Exists_ServiceExists()
        {
            var serviceName = "Spooler";

            var exists = WindowsServices.Exists(serviceName);

            exists.Should().BeTrue($"Service exists: {serviceName}");
        }

        [Fact]
        public void Exists_Exists_ServiceDoesNotExist()
        {
            var serviceName = "Exists_Exists_ServiceDoesNotExist";

            var exists = WindowsServices.Exists(serviceName);

            exists.Should().BeFalse($"Service exists: {serviceName}");
        }

        [Fact]
        public void Get_Status_StatusFromServicesIsRetrieved()
        {
            var serviceName = "Spooler";

            var isrunning = WindowsServices.Is(serviceName, WindowsServiceStatus.Running);

            isrunning.Should().BeTrue(serviceName);
        }
    }
}
