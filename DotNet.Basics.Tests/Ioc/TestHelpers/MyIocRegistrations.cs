using Autofac;
using DotNet.Basics.Ioc;

namespace DotNet.Basics.Tests.Ioc.TestHelpers
{
    public class MyIocRegistrations : IIocRegistrations
    {
        public void RegisterIn(ContainerBuilder builder)
        {
            builder.RegisterType<MyType1>().As<IMyType>();
        }
    }
}
