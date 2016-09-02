using System;
using System.Diagnostics;
using NUnit.Framework;

namespace DotNet.Basics.Tests
{
    [SetUpFixture]
    public class TestInit
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
        }
    }
}
