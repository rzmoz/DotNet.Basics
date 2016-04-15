using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Basics.Ioc;
using DotNet.Basics.Tests.Ioc.TestHelpers;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Ioc
{
    [TestFixture]
    public class IocContainerBindInstanceTests
    {
        private const string _bindingNameAlpha = "MyBindingAlpha";
        private const string _bindingNameBeta = "MyBindingBeta";

        [Test]
        public void BindInstance_GetByInterface_InstanceIsReturnedFromInterface()
        {
            using (IIocContainer container = new IocContainer())
            {
                var instance = new MyType1();

                container.BindInstance<IMyType>(instance);

                var result = container.Get<IMyType>();

                result.GetType().Should().Be<MyType1>();
            }
        }

        [Test]
        public void BindInstance_OverrideExistingRegistrationsInstance_NewInstanceIsResolved()
        {
            using (IIocContainer container = new IocContainer())
            {
                //arrange 
                const int result1 = 1;
                const int result2 = 2;
                var instance1 = Substitute.For<IMyType>();
                instance1.GetValue().Returns(result1);
                var instance2 = Substitute.For<IMyType>();
                instance2.GetValue().Returns(result2);
                //act
                container.BindInstance<IMyType>(instance1);
                var instance1Result = container.Get<IMyType>().GetValue();
                container.BindInstance<IMyType>(instance2);
                var instance2Result = container.Get<IMyType>().GetValue();
                //assert
                instance1Result.Should().Be(result1);
                instance2Result.Should().Be(result2);
            }
        }

        [Test]
        public void BindInstance_NamedInstanceOverride_DifferentNamesDontOverride()
        {
            using (IIocContainer container = new IocContainer())
            {
                //arrange 
                const int result1 = 1;
                const int result2 = 2;
                var instance1 = Substitute.For<IMyType>();
                instance1.GetValue().Returns(result1);
                var instance2 = Substitute.For<IMyType>();
                instance2.GetValue().Returns(result2);
                //act
                container.BindInstance<IMyType>(instance1);
                var resolvedInstance1 = container.Get<IMyType>();
                container.BindInstance<IMyType>(instance2, _bindingNameAlpha);
                var resolvedInstance2 = container.Get<IMyType>();
                //assert
                resolvedInstance1.GetValue().Should().Be(result1);
                resolvedInstance2.GetValue().Should().Be(result1);
            }
        }

        }
}
