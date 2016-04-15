using DotNet.Basics.Ioc;

namespace DotNet.Basics.Tests.Ioc.TestHelpers
{
    public class MyIocContainerRegistrations : IIocRegistrations
    {
        public void RegisterIn(IocContainer container)
        {
            container.RegisterCollection<IMyType>(new[] { typeof(MyType1), typeof(MyType2) });
        }
    }
}
