using System;
using System.Linq;
using System.ServiceProcess;
using DotNet.Basics.Tasks.Repeating;

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

        public static bool IsRunning(string serviceName)
        {
            using (var ctrl = Get(serviceName))
            {
                return ctrl?.Status == ServiceControllerStatus.Running;
            }
        }

        public static bool Start(string serviceName)
        {
            return Start(serviceName, DefaultTimeout);
        }
        public static bool Start(string serviceName, TimeSpan timeout)
        {
            return InvokeService(serviceName, service => service.Start(), ServiceControllerStatus.Running, timeout);
        }

        public static bool Stop(string serviceName)
        {
            return Stop(serviceName, DefaultTimeout);
        }
        public static bool Stop(string serviceName, TimeSpan timeout)
        {
            return InvokeService(serviceName, service => service.Stop(), ServiceControllerStatus.Stopped, timeout);
        }

        public static bool Restart(string serviceName)
        {
            return Restart(serviceName, DefaultTimeout);
        }
        public static bool Restart(string serviceName, TimeSpan timeout)
        {
            return Stop(serviceName, timeout) && Start(serviceName, timeout);
        }

        private static bool InvokeService(string serviceName, Action<ServiceController> serviceAction, ServiceControllerStatus exitStatus, TimeSpan timeout)
        {
            if (serviceName == null) throw new ArgumentNullException(nameof(serviceName));
            using (var service = Get(serviceName))
            {
                if (service == null)
                    return false;

                var success = Repeat.TaskOnce(() => serviceAction(service))
                    .WithOptions(o =>
                    {
                        o.RetryDelay = 1.Seconds();
                        o.Timeout = timeout;
                    }).Until(() => service.Status == exitStatus);
                return success;
            }
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
