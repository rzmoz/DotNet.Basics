using DotNet.Basics.Collections;
using SimpleInjector;

namespace DotNet.Basics.Ioc
{
    public class SimpleContainer : Container
    {
        public SimpleContainer(params ISimpleRegistrations[] registrations)
        {
            Options.AllowOverridingRegistrations = true;
            registrations?.ForEach(r => r.RegisterIn(this));
        }
    }
}
