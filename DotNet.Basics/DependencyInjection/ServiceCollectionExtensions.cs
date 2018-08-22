using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRegistrations(this IServiceCollection services, params IRegistrations[] registrations)
        {
            foreach (var registration in registrations)
            {
                registration.RegisterIn(services);
            }
        }
    }
}
