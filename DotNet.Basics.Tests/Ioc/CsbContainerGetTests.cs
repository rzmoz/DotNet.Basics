using System;
using DotNet.Basics.Ioc;
using DotNet.Basics.Tests.Ioc.TestHelpers;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Ioc
{
    [TestFixture]
    public class CsbContainerGetTests
    {
        private const string _bindingNameAlpha = "MyBindingAlpha";
        private const string _bindingNameBeta = "MyBindingBeta";
        
        [Test]
        public void Get_ReferenceChainInMultipleRegistrationTypes_ConstructorDependenciesAreResolved()
        {
            using (IDotNetContainer container = new DotNetContainer())
            {
                container.BindType<IInterfaceUsedInConstructor, ClassImplementingInterfaceUsedInConstructor>();
                container.BindType<IInterfaceForClassDependentOnInterfaceUsedInConstructor, ClassDependentOnInterfaceUsedInConstructor>();

                //get by interface
                var dependentObjectByInterface = container.Get<IInterfaceForClassDependentOnInterfaceUsedInConstructor>();
                dependentObjectByInterface.Should().NotBeNull();

                //get by specific class
                var dependentObjectByClass = container.Get<ClassDependentOnInterfaceUsedInConstructor>();
                dependentObjectByClass.Should().NotBeNull();
            }
        }

        [Test]
        public void Get_InterfaceIsntRegistered_ExceptionIsThrown()
        {
            using (IDotNetContainer container = new DotNetContainer())
            {
                //act
                Action act = () => container.Get<IMyType>(); //not registered
                //assert
                act.ShouldThrow<IocException>();
            }
        }

        [Test]
        public void Get_NamedTypeIsntRegistered_ExceptionIsThrown()
        {
            using (IDotNetContainer container = new DotNetContainer())
            {
                //act
                Action act = () => container.Get<IMyType>("named"); //not registered
                //assert
                act.ShouldThrow<IocException>();
            }
        }

        [Test]
        public void Get_TypeIsntRegistered_TypeIsResolved()
        {
            using (IDotNetContainer container = new DotNetContainer())
            {
                //act
                var type = container.Get<MyType1>(); //not registered
                type.GetType().Should().Be<MyType1>();
            }
        }

        [Test]
        public void Get_NestedNamedResolving_DependencyIsResolvedBasedOnNamedParameter()
        {
            using (IDotNetContainer container = new DotNetContainer())
            {
                //act
                container.BindInstance<IMyType>(new MyType1(), _bindingNameAlpha);
                container.BindInstance<IMyType>(new MyType2(), _bindingNameBeta);
                var type1 = container.Get<IMyType>(_bindingNameAlpha);
                var type2 = container.Get<IMyType>(_bindingNameBeta);
                //assert
                type1.GetType().Should().Be<MyType1>();
                type2.GetType().Should().Be<MyType2>();
            }
        }

        [Test]
        public void Get_WithNoNameWhenNamedTypeIsRegistered_FirstRegistrationIsReturnEvenIfNamed()
        {
            using (IDotNetContainer container = new DotNetContainer())
            {
                //act
                container.BindType<IMyType, MyType1>(_bindingNameAlpha);
                var type = container.Get<IMyType>(); //we get default without name
                //assert
                type.GetType().Should().Be(typeof(MyType1));
            }
        }

        [Test]
        public void Get_WithNoNameWhenNamedInstanceIsRegistered_FirstRegistrationIsReturnEvenIfName()
        {
            using (IDotNetContainer container = new DotNetContainer())
            {
                //arrange
                var instance = new MyType1();
                //act
                container.BindInstance<IMyType>(instance, _bindingNameAlpha);
                var type = container.Get<IMyType>(); //we get default without name
                //assert
                type.GetType().Should().Be(instance.GetType());
            }
        }

        [Test]
        public void Get_WithNotRegisteredNameWhenNamedTypeIsRegistered_ExceptionIsThrown()
        {
            using (IDotNetContainer container = new DotNetContainer())
            {
                //act
                container.BindType<IMyType, MyType1>(_bindingNameAlpha);
                Action action = () => container.Get<IMyType>("SomethingThatIsNotRegistered"); //we get default
                //assert
                action.ShouldThrow<IocException>();
            }
        }

        [Test]
        public void Get_WithNotRegisteredNameWhenNamedInstanceIsRegistered_ExceptionIsThrown()
        {
            using (IDotNetContainer container = new DotNetContainer())
            {
                //act
                container.BindInstance<IMyType>(new MyType1(), _bindingNameAlpha);
                Action action = () => container.Get<IMyType>("SomethingThatIsNotRegistered"); //we get default
                //assert
                action.ShouldThrow<IocException>();
            }
        }
    }
}
