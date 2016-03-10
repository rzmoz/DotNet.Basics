using System;
using System.Linq;
using DotNet.Basics.Ioc;
using DotNet.Basics.Tests.Ioc.TestHelpers;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Ioc
{
    [TestFixture]
    public class CsbContainerRegistrationsTests
    {
        private DotNetContainer _container;

        [SetUp]
        public void SetUp()
        {
            _container = new DotNetContainer();
        }

        [Test]
        public void _AddRegistration_DerivedCsbContainerType_RegistrationsAreAdded()
        {
            var myCsbRegistrations = new MyDotNetContainerRegistrations();
            MyDotNetContainer container = new MyDotNetContainer();

            container.Registrations.Count().Should().Be(0);

            Action action = () => container.Get<IMyType>();
            action.ShouldThrow<IocException>();
            container.Registrations.Add(myCsbRegistrations);

            container.Registrations.Count().Should().Be(1);

            var resolved = container.Get<IMyType>();
            resolved.Should().NotBeNull();
        }

        [Test]
        public void Reset_DiscardMocks_AllRegistrationsNotInRegistrationsAreLost()
        {
            //arrange 
            _container.BindInstance<IMyType>(Substitute.For<IMyType>());
            //act
            var resolvesBeforeReset = _container.Get<IMyType>();
            _container.Reset();
            //assert
            resolvesBeforeReset.Should().NotBeNull();
            _container.Invoking(c => c.Get<IMyType>()).ShouldThrow<IocException>();
        }

        [Test]
        public void AddRegistration_AddingSameRegistration_RegistrationsAreOnlyAddedOnce()
        {
            _container.Registrations.Add<MyTypeRegistrations>();
            _container.Registrations.Add<MyTypeRegistrations>();
            _container.Registrations.Add<MyTypeRegistrations>();

            RegistrationsAreOnlyAddedOnceAssert(_container);
        }

        [Test]
        public void AddRegistrations_AddingSameRegistration_RegistrationsAreOnlyAddedOnce()
        {
            _container.Registrations.Add(new MyTypeRegistrations());
            _container.Registrations.Add(new MyTypeRegistrations());
            _container.Registrations.Add(new MyTypeRegistrations());

            RegistrationsAreOnlyAddedOnceAssert(_container);
        }

        private void RegistrationsAreOnlyAddedOnceAssert(DotNetContainer container)
        {
            container.Registrations.Count().Should().Be(1);//MyTypeRegistrations contain two registrations
        }
    }
}
