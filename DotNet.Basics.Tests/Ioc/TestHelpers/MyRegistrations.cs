using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Basics.Ioc;

namespace DotNet.Basics.Tests.Ioc.TestHelpers
{
    public class MyRegistrations : ISimpleRegistrations
    {
        public void RegisterIn(SimpleContainer container)
        {
            container.Register<IMyType, MyType1>();
        }
    }
}
