using System;

namespace DotNet.Basics.Tests.Cli
{
    public class MyServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}
