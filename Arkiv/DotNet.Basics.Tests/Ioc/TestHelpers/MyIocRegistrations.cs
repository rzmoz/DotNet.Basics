using Autofac;
using DotNet.Basics.Ioc;

namespace DotNet.Basics.Tests.Ioc.TestHelpers
{
    public class MyIocRegistrations : IIocRegistrations
    {
        public void RegisterIn(IocBuilder builder)
        {
            builder.RegisterType<MyType1>().As<IMyType>();
            builder.Register(t => new TypeThatDependesOnContainer(builder.Container)).AsSelf();
        }
    }
}
