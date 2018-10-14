﻿using Xunit;
using DotNet.Basics.Reflection;
using FluentAssertions;

namespace DotNet.Basics.Tests.Reflection
{
    public class ReflectionExtensionsTests
    {
        [Fact]
        public void IsBaseClassOf_GenericConstraint_BaseClassIsMatched()
        {
            var result = typeof(MyBaseClass<>).IsBaseClassOf<MyDerived2ndOrderClass>();
            result.Should().BeTrue();
        }

        public class MyBaseClass<T> { }

        public class MyDerivedClass<T> : MyBaseClass<T> { }

        public class MyDerived2ndOrderClass : MyDerivedClass<int> { }
    }
}
