﻿using DotNet.Basics.Ioc;

namespace DotNet.Basics.Tests.Ioc.TestHelpers
{
    public class MyTypeRegistrations : IDotNetRegistrations
    {
        public void RegisterIn(IIocContainer container)
        {
            container.BindType<IMyType, MyType1>(typeof(MyType1).ToString());
            container.BindType<IMyType, MyType2>(typeof(MyType2).ToString());
        }
    }
}
