using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Basics.Ioc;

namespace DotNet.Basics.Tests.Ioc.TestHelpers
{
    public class MyRegistrations : IIocRegistrations
    {
        public void RegisterIn(IocContainer container)
        {
            container.Register<IMyType, MyType1>();
        }
    }
}
