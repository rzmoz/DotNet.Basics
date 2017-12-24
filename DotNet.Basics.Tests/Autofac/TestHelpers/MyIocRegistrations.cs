using Autofac;
using DotNet.Basics.Autofac;

namespace DotNet.Basics.Tests.Autofac.TestHelpers
{
    public class MyIocRegistrations : IRegistrations
    {
        public void RegisterIn(AutofacBuilder builder)
        {
            builder.RegisterType<MyType1>().As<IMyType>();
        }
    }
}
