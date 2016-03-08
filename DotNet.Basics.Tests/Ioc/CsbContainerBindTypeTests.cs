using System;
using DotNet.Basics.Ioc;
using DotNet.Basics.Tests.Ioc.TestHelpers;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Ioc
{
    [TestFixture]
    public class CsbContainerBindTypeTests
    {
        private const string _bindingNameAlpha = "MyBindingAlpha";

        [Test]
        public void BindType_BindNonGenericWithTypeNotImplementingInterface_RuntimeException()
        {
            using (ICsbContainer container = new CsbContainer())
            {
                //act
                Action act = () => container.BindType<IMyType>(typeof(CsbContainerBindTypeTests));

                act.ShouldThrow<ArgumentException>();
            }
        }

        [Test]
        public void BindType_BindNonGenericWithBothInterfaceAndTyp_BindingsAreResolved()
        {
            using (ICsbContainer container = new CsbContainer())
            {
                //act
                container.BindType<IMyType>(typeof(MyType1));
                var byInterface = container.Get<IMyType>();
                var byType = container.Get<MyType1>();

                //assert
                byInterface.GetType().Should().Be<MyType1>();
                byType.GetType().Should().Be<MyType1>();
            }
        }
        [Test]
        public void BindType_BindNonGenericWithNaming_BindingsAreResolved()
        {
            using (ICsbContainer container = new CsbContainer())
            {
                //act
                container.BindType<IMyType>(typeof(MyType1),_bindingNameAlpha);
                var byInterface = container.Get<IMyType>(_bindingNameAlpha);
                var byType = container.Get<MyType1>(_bindingNameAlpha);

                //assert
                byInterface.GetType().Should().Be<MyType1>();
                byType.GetType().Should().Be<MyType1>();
            }
        }

        [Test]
        public void BindType_BindNonGeneric_BindingsAreResolvedByMode()
        {
            using (ICsbContainer container = new CsbContainer())
            {
                //act
                container.BindType<IMyType>(typeof(MyType1));
                var type1 = container.Get<IMyType>();
                container.BindType<IMyType>(typeof(MyType2), mode: IocMode.Debug);
                var type2Live = container.Get<IMyType>();
                var type2Debug = container.Get<IMyType>(IocMode.Debug);

                //assert
                type1.GetType().Should().Be<MyType1>();
                type2Live.GetType().Should().Be<MyType1>();
                type2Debug.GetType().Should().Be<MyType2>();
            }
        }

        [Test]
        public void BindType_BindNonGeneric_NewRegistrationsAreAdded()
        {
            using (ICsbContainer container = new CsbContainer())
            {
                //act
                container.BindType<IMyType>(typeof(MyType1));
                var type1 = container.Get<IMyType>();
                container.BindType<IMyType>(typeof(MyType2));
                var type2 = container.Get<IMyType>();

                //assert
                type1.GetType().Should().Be<MyType1>();
                type2.GetType().Should().Be<MyType2>();
            }
        }

        [Test]
        public void BindType_BindTypesAfterContainerIsCreated_NewRegistrationsAreAdded()
        {
            using (ICsbContainer container = new CsbContainer())
            {
                //act
                container.BindType<IMyType, MyType1>();
                var type1 = container.Get<IMyType>();
                container.BindType<IMyType, MyType2>();
                var type2 = container.Get<IMyType>();
                //assert
                type1.GetType().Should().Be<MyType1>();
                type2.GetType().Should().Be<MyType2>();
            }
        }

        [Test]
        public void BindType_OverrideExistingRegistrationsType_NewTypeIsResolved()
        {
            using (ICsbContainer container = new CsbContainer())
            {
                //act
                container.BindType<IMyType, MyType1>();
                var type1 = container.Get<IMyType>(IocMode.Synthetic);
                container.BindType<IMyType, MyType2>();
                var type2 = container.Get<IMyType>(IocMode.Synthetic);
                //assert
                type1.GetType().Should().Be<MyType1>();
                type2.GetType().Should().Be<MyType2>();
            }
        }

        [Test]
        public void BindType_NamedTypeOverride_DifferentNamesDontOverride()
        {
            using (ICsbContainer container = new CsbContainer())
            {
                //act
                container.BindType<IMyType, MyType1>();
                var resolvedInstance1 = container.Get<IMyType>(IocMode.Synthetic);
                //same behavior is expected in chained container
                container.BindType<IMyType, MyType2>(_bindingNameAlpha);
                var resolvedInstance2 = container.Get<IMyType>(IocMode.Synthetic);
                //same behavior is expected in chained container
                //assert
                resolvedInstance1.GetType().Should().Be(typeof(MyType1));
                resolvedInstance2.GetType().Should().Be(typeof(MyType1));
            }
        }

        [Test]
        public void BindType_NamedTypeOverride_SameNameOverrides()
        {
            using (ICsbContainer container = new CsbContainer())
            {
                //act
                container.BindType<IMyType, MyType1>(_bindingNameAlpha);
                var resolvedInstance1 = container.Get<IMyType>(_bindingNameAlpha, IocMode.Synthetic);
                //same behavior is expected in chained container
                container.BindType<IMyType, MyType2>(_bindingNameAlpha);
                var resolvedInstance2 = container.Get<IMyType>(_bindingNameAlpha, IocMode.Synthetic);
                //same behavior is expected in chained container
                //assert
                resolvedInstance1.GetType().Should().Be(typeof(MyType1));
                resolvedInstance2.GetType().Should().Be(typeof(MyType2));
            }
        }
    }
}
