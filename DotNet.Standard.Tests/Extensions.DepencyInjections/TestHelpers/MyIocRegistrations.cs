using Autofac;
using DotNet.Standard.Extensions.DependencyInjection;

namespace DotNet.Standard.Tests.Extensions.DepencyInjections.TestHelpers
{
    public class MyIocRegistrations : IRegistrations
    {
        public void RegisterIn(ContainerBuilder builder)
        {
            builder.RegisterType<MyType1>().As<IMyType>();
        }
    }
}
