using DotNet.Basics.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Basics.Tests.Autofac.TestHelpers
{
    public class MyAutofacRegistrations : IConfigureServices
    {
        public void Configure(IServiceCollection services)
        {
            services.AddTransient<IMyType, MyType1>();
        }
    }
}
