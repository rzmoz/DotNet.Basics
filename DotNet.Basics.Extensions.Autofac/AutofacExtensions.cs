using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Extensions.Autofac
{
    public static class AutofacExtensions
    {
        public static IServiceCollection AddAutofac(this IServiceCollection services, params IAutofacRegistrations[] registrations)
        {
            return services.AddAutofac(true, registrations);
        }
        public static IServiceCollection AddAutofac(this IServiceCollection services, bool resolveConcreteTypesNotAlreadyRegistered, params IAutofacRegistrations[] registrations)
        {
            return services.AddSingleton<IServiceProviderFactory<ContainerBuilder>>(new AutofacBuilderServicesProviderFactory(registrations, resolveConcreteTypesNotAlreadyRegistered));
        }

        public static void Register(this AutofacBuilder builder, params IAutofacRegistrations[] registrations)
        {
            foreach (IAutofacRegistrations t in registrations)
                t.RegisterIn(builder);
        }
    }
}
