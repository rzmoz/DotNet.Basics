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

        public static bool Is(string serviceName, WindowsServiceStatus status)
        {
            using (var ctrl = Get(serviceName))
            {
                return (int)ctrl?.Status == (int)status;
            }
        }

        public static bool Start(string serviceName)
        {
            return Start(serviceName, DefaultTimeout);
        }
        public static bool Start(string serviceName, TimeSpan timeout)
        {
            return InvokeService(serviceName, service => service.Start(), WindowsServiceStatus.Running, timeout);
        }

        public static bool Stop(string serviceName)
        {
            return Stop(serviceName, DefaultTimeout);
        }
        public static bool Stop(string serviceName, TimeSpan timeout)
        {
            return InvokeService(serviceName, service => service.Stop(), WindowsServiceStatus.Stopped, timeout);
        }

        public static bool Restart(string serviceName)
        {
            return Restart(serviceName, DefaultTimeout);
        }
        public static bool Restart(string serviceName, TimeSpan timeout)
        {
            return Stop(serviceName, timeout) && Start(serviceName, timeout);
        }

        private static bool InvokeService(string serviceName, Action<ServiceController> serviceAction, WindowsServiceStatus exitStatus, TimeSpan timeout)
        {
            if (serviceName == null) throw new ArgumentNullException(nameof(serviceName));
            if (Exists(serviceName) == false)
                return false;

            ServiceController service = Get(serviceName);
            var success = Repeat.TaskOnce(() => serviceAction(service))
                .WithOptions(o =>
                {
                    o.RetryDelay = 1.Seconds();
                    o.Timeout = timeout;
                    o.PingOnRetry = () => service = Get(serviceName);
                    o.Finally = () => service.Close();
                }).Until(() => (int)service.Status == (int)exitStatus);
            service = null;
            return success;

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
