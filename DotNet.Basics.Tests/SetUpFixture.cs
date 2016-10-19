using System;
using NUnit.Framework;

namespace DotNet.Basics.Tests
{
    [SetUpFixture]
    public class SetUpFixture
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            DebugOut.Out += Console.WriteLine;
        }
    }
}
