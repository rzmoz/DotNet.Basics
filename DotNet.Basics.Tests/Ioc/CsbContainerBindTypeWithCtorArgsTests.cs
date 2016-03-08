using DotNet.Basics.Ioc;
using DotNet.Basics.Tests.Ioc.TestHelpers;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Ioc
{
    [TestFixture]
    public class CsbContainerBindTypeWithCtorArgsTests
    {
        private ICsbContainer _container;

        [SetUp]
        public void SetUp()
        {
            _container = new CsbContainer();
        }

        [Test]
        public void BindTypeWithCtorArgs_SingleTypeEagerArg_TypeIsResolved()
        {
            //arrange 
            const string nameConstructorArgumentValue = "MyText";
            //act
            _container.BindTypeWithCtorArgs<TypeWithStringParameterInConstructor>(new EagerCtorArg("text", nameConstructorArgumentValue));

            var type = _container.Get<TypeWithStringParameterInConstructor>();
            //assert
            type.Text.Should().Be(nameConstructorArgumentValue);
        }

        [Test]
        public void BindTypeWithCtorArgs_SingleTypeLazyArg_TypeIsResolved()
        {
            //arrange 
            const string nameConstructorArgumentValue = "MyText";
            //act
            _container.BindTypeWithCtorArgs<TypeWithStringParameterInConstructor>(new LazyCtorArg("text", () => nameConstructorArgumentValue));

            var type = _container.Get<TypeWithStringParameterInConstructor>();
            //assert
            type.Text.Should().Be(nameConstructorArgumentValue);
        }

        [Test]
        public void BindTypeWithCtorArgs_InterfaceAndTypeEagerArg_TypeIsResolved()
        {
            //arrange 
            const string nameConstructorArgumentValue = "MyText";
            //act
            _container.BindTypeWithCtorArgs<ITypeWithStringParameterInConstructor, TypeWithStringParameterInConstructor>(new EagerCtorArg("text", nameConstructorArgumentValue));

            var type = _container.Get<ITypeWithStringParameterInConstructor>();
            _container.Get<TypeWithStringParameterInConstructor>();//we don't want an exception thrown when getting by imp type
            //assert
            type.Text.Should().Be(nameConstructorArgumentValue);
        }

        [Test]
        public void BindTypeWithCtorArgs_InterfaceAndTypeLazyArg_TypeIsResolved()
        {
            //arrange 
            const string nameConstructorArgumentValue = "MyText";
            //act
            _container.BindTypeWithCtorArgs<ITypeWithStringParameterInConstructor, TypeWithStringParameterInConstructor>(new LazyCtorArg("text", () => nameConstructorArgumentValue));

            var type = _container.Get<ITypeWithStringParameterInConstructor>();
            _container.Get<TypeWithStringParameterInConstructor>();//we don't want an exception thrown when getting by imp type
            //assert
            type.Text.Should().Be(nameConstructorArgumentValue);
        }
    }
}
