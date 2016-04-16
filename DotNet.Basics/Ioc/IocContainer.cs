using DotNet.Basics.Collections;
using SimpleInjector;

namespace DotNet.Basics.Ioc
{
    public class IocContainer : Container
    {
        public IocContainer(params IIocRegistrations[] registrations)
            : base()
        {
            Options.AllowOverridingRegistrations = true;
            registrations?.ForEach(r => r.RegisterIn(this));
        }
    }
}
