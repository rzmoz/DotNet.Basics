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
    public class CsbContainerBindInstanceTests
    {
        private const string _bindingNameAlpha = "MyBindingAlpha";
        private const string _bindingNameBeta = "MyBindingBeta";

        [Test]
        public void BindInstance_GetByInterface_InstanceIsReturnedFromInterface()
        {
            using (ICsbContainer container = new CsbContainer())
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
            using (ICsbContainer container = new CsbContainer())
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
                var instance1Result = container.Get<IMyType>(IocMode.Synthetic).GetValue();
                container.BindInstance<IMyType>(instance2);
                var instance2Result = container.Get<IMyType>(IocMode.Synthetic).GetValue();
                //assert
                instance1Result.Should().Be(result1);
                instance2Result.Should().Be(result2);
            }
        }

        [Test]
        public void BindInstance_NamedInstanceOverride_DifferentNamesDontOverride()
        {
            using (ICsbContainer container = new CsbContainer())
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
                var resolvedInstance1 = container.Get<IMyType>(IocMode.Synthetic);
                container.BindInstance<IMyType>(instance2, _bindingNameAlpha);
                var resolvedInstance2 = container.Get<IMyType>(IocMode.Synthetic);
                //assert
                resolvedInstance1.GetValue().Should().Be(result1);
                resolvedInstance2.GetValue().Should().Be(result1);
            }
        }


        [Test]
        public void BindInstance_OverrideExistingTypeWithInstanceFromLiveInSynthetic_NewInstanceIsResolved()
        {
            using (ICsbContainer container = new CsbContainer())
            {
                //arrange 
                const int expectedResult = 100;
                var instance = Substitute.For<IMyType>();
                instance.GetValue().Returns(expectedResult);
                //act
                container.BindType<IMyType, MyType1>(mode: IocMode.Live); //from default live registration
                container.BindInstance<IMyType>(instance, mode: IocMode.Synthetic);
                //we want to add an override in synthetic
                var liveResult = container.Get<IMyType>(IocMode.Live).GetValue(); //
                var testResult = container.Get<IMyType>(IocMode.Synthetic).GetValue(); //
                //assert
                liveResult.Should().Be(1);
                testResult.Should().Be(expectedResult);
            }
        }

        [Test]
        public void BindInstance_OverrideExistingReferencedTypeFromLiveInSynthetic_InstanceIsResolvedWithOverridenInstance()
        {
            using (ICsbContainer container = new CsbContainer())
            {
                //arrange 
                const int expectedResult = 100;
                var instance = Substitute.For<IMyType>();
                instance.GetValue().Returns(expectedResult);
                //act
                container.BindType<DependsOnIMyType, DependsOnIMyType>();
                container.BindType<IMyType, MyType1>(mode: IocMode.Live); //from default live registration
                container.BindInstance<IMyType>(instance, mode: IocMode.Synthetic);
                //we want to add an override in test
                var result = container.Get<DependsOnIMyType>(IocMode.Synthetic).GetValueFromIMyType(); //
                //assert
                result.Should().Be(expectedResult); //result from live container
            }
        }
    }
}
