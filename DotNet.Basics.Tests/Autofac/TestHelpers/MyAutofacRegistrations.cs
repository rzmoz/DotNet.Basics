using Autofac;
using DotNet.Basics.Autofac;

namespace DotNet.Basics.Tests.Autofac.TestHelpers
{
    public class MyAutofacRegistrations : IAutofacRegistrations
    {
        public void RegisterIn(ContainerBuilder builder)
        {
            builder.RegisterType<MyType1>().As<IMyType>();
        }
    }
}
