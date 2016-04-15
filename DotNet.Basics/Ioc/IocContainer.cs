using SimpleInjector;

namespace DotNet.Basics.Ioc
{
    public class IocContainer : Container
    {
        public IocContainer()
            : base()
        {
            Options.AllowOverridingRegistrations = true;
        }
    }
}
