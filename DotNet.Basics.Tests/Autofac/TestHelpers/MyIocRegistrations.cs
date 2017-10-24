using Autofac;
using DotNet.Basics.Extensions.Autofac;

namespace DotNet.Basics.Tests.AutoFac.TestHelpers
{
    public class MyIocRegistrations : IAutofacRegistrations
    {
        public void RegisterIn(AutofacBuilder builder)
        {
            builder.RegisterType<MyType1>().As<IMyType>();
            builder.Register(t => new TypeThatDependesOnContainer(builder.Container)).AsSelf();
        }
    }
}
