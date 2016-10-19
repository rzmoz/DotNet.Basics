using System;
using Autofac;
using Autofac.Core.Registration;
using DotNet.Basics.AppSettings;
using DotNet.Basics.Ioc;
using DotNet.Basics.Tests.Ioc.TestHelpers;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Ioc
{
    [TestFixture]
    public class IocBuilderTests
    {
        private IocBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new IocBuilder();
        }

        [Test]
        public void Register_TypeThatDependsOnContainer_TypeIsResolved()
        {
            _builder.Register(new MyIocRegistrations());
            var mytype = _builder.Container.Resolve<IMyType>();
            var typeThatDependesOnContainer = _builder.Container.Resolve<TypeThatDependesOnContainer>();
            typeThatDependesOnContainer.Value.Should().Be(mytype.GetValue());
        }

        [Test]
        public void Build_Registrations_RegistrationsAreRegistered()
        {
            var container = _builder.Build();
            Action getWithoutRegistrions = () => { container.Resolve<IMyType>(); };
            getWithoutRegistrions.ShouldThrow<ComponentNotRegisteredException>();
        }

        [Test]
        public void Ctor_Registrations_RegistrationsAreRegistered()
        {
            _builder.Register(new MyIocRegistrations());
            var myResolvedType = _builder.Build().Resolve<IMyType>();
            myResolvedType.GetType().Should().Be<MyType1>();
        }

        [Test]
        public void GetInstance_Singleton_InstanceIsResolved()
        {
            const int before = 1;
            const int update = 5;

            var type = new TypeWithValue
            {
                Value = before
            };
            _builder.RegisterInstance(type);
            var container = _builder.Build();

            var resolved1 = container.Resolve<TypeWithValue>();
            resolved1.Value.Should().Be(type.Value);
            type.Value = update;

            //assert instance is updated
            var resolved2 = container.Resolve<TypeWithValue>();
            resolved2.Value.Should().Be(type.Value);
        }

        [Test]
        public void GetInstance_ByInterface_InstanceIsResolved()
        {
            _builder.RegisterType<MyType1>().As<IMyType>().AsSelf();
            var container = _builder.Build();

            var resolvedByInterface = container.Resolve<IMyType>();
            var resolvedByImplementation = container.Resolve<MyType1>();

            resolvedByInterface.GetType().Should().Be<MyType1>();
            resolvedByImplementation.GetType().Should().Be<MyType1>();
        }

        [Test]
        public void GetInstance_UnregisteredDefaultConstructor_InstanceIsResolved()
        {
            using (var container = _builder.Build())
            {
                var resolvedByImplementation = container.Resolve<MyType1>();

                resolvedByImplementation.GetType().Should().Be<MyType1>();
            }
        }
    }
}
