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

        public new void Verify()
        {
            base.Verify();
        }
        public new void Verify(VerificationOption vo)
        {
            base.Verify(vo);
        }
    }
}
