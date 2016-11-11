using System;
using System.Linq;
using System.ServiceProcess;

namespace DotNet.Basics.Sys
{
    public static class WindowsServices
    {
        public static TimeSpan DefaultTimeout => 30.Seconds();

        public static bool Exists(string serviceName)
        {
            if (serviceName == null) throw new ArgumentNullException(nameof(serviceName));
            return
                ServiceController.GetServices()
                    .Any(ctrl => ctrl.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
        }

        public static bool Is(string serviceName, WindowsServiceStatus status)
        {
            using (var ctrl = Get(serviceName))
            {
                return (int)ctrl?.Status == (int)status;
            }
        }

        public static bool Start(string serviceName)
        {
            return CommandPrompt.Run($"net start {serviceName}") == 0;
        }

        public static bool Stop(string serviceName)
        {
            return CommandPrompt.Run($"net stop {serviceName}") == 0;
        }

        public static bool Restart(string serviceName)
        {
            return Stop(serviceName) && Start(serviceName);
        }

        private static ServiceController Get(string serviceName)
        {
            if (serviceName == null) throw new ArgumentNullException(nameof(serviceName));
            var service = ServiceController.GetServices().FirstOrDefault(ctrl => ctrl.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
            if (service == null)
                DebugOut.WriteLine($"WindowsService not found: {serviceName}");
            return service;
        }
    }
}
