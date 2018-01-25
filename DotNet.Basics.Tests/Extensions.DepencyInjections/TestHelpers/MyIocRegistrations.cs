using Autofac;
using DotNet.Basics.Extensions.DependencyInjection;

namespace DotNet.Basics.Tests.Extensions.DepencyInjections.TestHelpers
{
    public class MyIocRegistrations : IRegistrations
    {
        public void RegisterIn(ContainerBuilder builder)
        {
            builder.RegisterType<MyType1>().As<IMyType>();
        }
    }
}
