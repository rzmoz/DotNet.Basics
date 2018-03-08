using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Tests.DependencyInjection
{
    public class MyServiceCollectionRegistrations : IRegistrations
    {
        public void RegisterIn(IServiceCollection services)
        {
            services.AddTransient<IMyType, MyType1>();
        }
    }
}
