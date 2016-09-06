using System;
using DotNet.Basics.Ioc;
using DotNet.Basics.Tests.Ioc.TestHelpers;
using FluentAssertions;
using NUnit.Framework;
using SimpleInjector;

namespace DotNet.Basics.Tests.Ioc
{
    [TestFixture]
    public class IocContainerTests
    {
        [Test]
        public void Ctor_Registrations_RegistrationsAreRegistered()
        {
            var containerWitoutRegistrations = new SimpleContainer();

            Action getWithoutRegistrions = () => { var mytype = containerWitoutRegistrations.GetInstance<IMyType>(); };
            getWithoutRegistrions.ShouldThrow<ActivationException>();

            var containerWithRegistrations = new SimpleContainer(new MyRegistrations());
            var myResolvedType= containerWithRegistrations.GetInstance<IMyType>();
            myResolvedType.GetType().Should().Be<MyType1>();
        }

        [Test]
        public void GetInstance_Singleton_InstanceIsResolved()
        {
            using (var container = new SimpleContainer())
            {
                const int before = 1;
                const int update = 5;

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

        [Test]
        public void GetInstance_ByInterface_InstanceIsResolved()
        {
            using (var container = new SimpleContainer())
            {
                container.Register<IMyType, MyType1>();
                container.Verify();

                var resolvedByInterface = container.GetInstance<IMyType>();
                var resolvedByImplementation = container.GetInstance<MyType1>();

                resolvedByInterface.GetType().Should().Be<MyType1>();
                resolvedByImplementation.GetType().Should().Be<MyType1>();
            }
        }

        [Test]
        public void GetInstance_UnregisteredDefaultConstructor_InstanceIsResolved()
        {
            using (var container = new SimpleContainer())
            {
                var resolvedByImplementation = container.GetInstance<MyType1>();

                resolvedByImplementation.GetType().Should().Be<MyType1>();
            }
        }
    }
}
