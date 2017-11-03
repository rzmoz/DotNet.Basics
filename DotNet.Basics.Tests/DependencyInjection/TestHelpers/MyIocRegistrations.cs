using DotNet.Basics.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Tests.DependencyInjection.TestHelpers
{
    public class MyIocRegistrations : IRegistrations
    {
        public void RegisterIn(IServiceCollection services)
        {
            services.AddTransient<IMyType, MyType1>();
        }
    }
}
