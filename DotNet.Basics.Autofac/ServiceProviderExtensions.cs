using System;

namespace DotNet.Basics.Autofac
{
    public static class ServiceProviderExtensions
    {
        public static T GetService<T>(this IServiceProvider serviceProvider)
        {
            var instance = serviceProvider.GetService(typeof(T));
            if (instance == null)
                throw new ServiceNotResolvedException(typeof(T).FullName);
            return (T)instance;
        }
    }
}
