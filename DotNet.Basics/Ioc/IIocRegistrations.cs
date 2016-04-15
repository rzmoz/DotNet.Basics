using SimpleInjector;

namespace DotNet.Basics.Ioc
{
    public interface IIocRegistrations
    {
        void RegisterIn(IocContainer container);
    }
}
