using DotNet.Basics.Collections;
using SimpleInjector;

namespace DotNet.Basics.Ioc
{
    public class SimpleContainer : Container
    {
        public SimpleContainer()
        {
            Options.AllowOverridingRegistrations = true;
        }

        public void Register(params ISimpleRegistrations[] registrations)
        {
            registrations?.ForEach(r => r.RegisterIn(this));
        }
    }
}
