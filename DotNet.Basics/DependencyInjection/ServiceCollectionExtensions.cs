namespace Microsoft.Extensions.DependencyInjection
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
