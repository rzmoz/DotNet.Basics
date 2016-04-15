using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Basics.Ioc;
using DotNet.Basics.Tests.Ioc.TestHelpers;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Ioc
{
    [TestFixture]
    public class IocContainerTests
    {
        [Test]
        public void GetInstance_Singleton_InstanceIsResolved()
        {
            using (var container = new IocContainer())
            {
                const int before = 1;
                const int update= 5;

                var type = new TypeWithValue
                {
                    Value = before
                };
                container.RegisterSingleton(type);
                container.Verify();

                var resolved1 = container.GetInstance<TypeWithValue>();
                resolved1.Value.Should().Be(type.Value);
                type.Value = update;
                
                //assert instance is updated
                var resolved2 = container.GetInstance<TypeWithValue>();
                resolved2.Value.Should().Be(type.Value);
            }
        }
    }
}
