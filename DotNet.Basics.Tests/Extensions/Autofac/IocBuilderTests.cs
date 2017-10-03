using System;
using Autofac;
using Autofac.Core.Registration;
using DotNet.Basics.Extensions.Autofac;
using DotNet.Basics.Tests.Extensions.AutoFac.TestHelpers;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Extensions.AutoFac
{
    public class IocBuilderTests
    {
        private readonly AutofacBuilder _builder;

        public IocBuilderTests()
        {
            _builder = new AutofacBuilder();
        }

        [Fact]
        public void Ctor_ResolveConcreteTypesNotRegisteredFalse_ExceptionIsThrown()
        {
            var build = new AutofacBuilder(false);

            Action action = () => build.Container.Resolve<MyType1>();

            action.ShouldThrow<ComponentNotRegisteredException>().WithMessage("The requested service 'DotNet.Basics.Tests.MyType1' has not been registered. To avoid this exception, either register a component to provide the service, check for service registration using IsRegistered(), or use the ResolveOptional() method to resolve an optional dependency.");
        }

        [Fact]
        public void Ctor_ResolveConcreteTypesNotRegisteredDefault_ConcreteTypesAreResolved()
        {
            var build = new AutofacBuilder();

            Action action = () => build.Container.Resolve<MyType1>();

            action.ShouldNotThrow<ComponentNotRegisteredException>();
        }

        [Fact]
        public void Register_TypeThatDependsOnContainer_TypeIsResolved()
        {
            new MyIocRegistrations().RegisterIn(_builder);
            var mytype = _builder.Container.Resolve<IMyType>();
            var typeThatDependesOnContainer = _builder.Container.Resolve<TypeThatDependesOnContainer>();
            typeThatDependesOnContainer.Value.Should().Be(mytype.GetValue());
        }

        [Fact]
        public void Build_Registrations_RegistrationsAreRegistered()
        {
            var container = _builder.Build();
            Action getWithoutRegistrions = () => { container.Resolve<IMyType>(); };
            getWithoutRegistrions.ShouldThrow<ComponentNotRegisteredException>();
        }

        [Fact]
        public void Ctor_Registrations_RegistrationsAreRegistered()
        {
            new MyIocRegistrations().RegisterIn(_builder);
            var myResolvedType = _builder.Build().Resolve<IMyType>();
            myResolvedType.GetType().Should().Be<MyType1>();
        }

        [Fact]
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

        [Fact]
        public void GetInstance_ByInterface_InstanceIsResolved()
        {
            _builder.RegisterType<MyType1>().As<IMyType>().AsSelf();
            var container = _builder.Build();

            var resolvedByInterface = container.Resolve<IMyType>();
            var resolvedByImplementation = container.Resolve<MyType1>();

            resolvedByInterface.GetType().Should().Be<MyType1>();
            resolvedByImplementation.GetType().Should().Be<MyType1>();
        }

        [Fact]
        public void GetInstance_UnregisteredDefaultConstructor_InstanceIsResolved()
        {
            using (var container = new AutofacBuilder(true).Container)
            {
                var resolvedByImplementation = container.Resolve<MyType1>();

                resolvedByImplementation.GetType().Should().Be<MyType1>();
            }
        }
    }
}
