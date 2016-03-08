using System;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Sys
{
    [TestFixture]
    public class TypeExtensionsTests
    {
        [Test]
        public void Is_IsType_TypeIsString()
        {
            var testType = typeof(string);
            testType.Is<string>().Should().BeTrue();
        }
        [Test]
        public void Is_NullHandling_TypeIsfalse()
        {
            Type testType = null;
            testType.Is<string>().Should().BeFalse();
        }
    }
}
