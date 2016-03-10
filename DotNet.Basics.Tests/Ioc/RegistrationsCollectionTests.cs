using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Basics.Ioc;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Ioc
{
    [TestFixture]
    public class RegistrationsCollectionTests
    {
        [Test]
        public void Add_AddInOrderBFirst_RegistrationsAreEnumeratedFIFO()
        {
            var registrations = new RegistrationsCollection(new DotNetContainer())
            {
                new BRegistrations(),//Add B first to make sure it stays first
                new ARegistrations()
            };

            registrations.ToList().First().GetType().Should().Be<BRegistrations>();
            registrations.ToList().Last().GetType().Should().Be<ARegistrations>();
        }
        [Test]
        public void Add_AddInOrderAFirst_RegistrationsAreEnumeratedFIFO()
        {
            var registrations = new RegistrationsCollection(new DotNetContainer())
            {
                new ARegistrations(),
                new BRegistrations()
            };

            registrations.ToList().First().GetType().Should().Be<ARegistrations>();
            registrations.ToList().Last().GetType().Should().Be<BRegistrations>();
        }
    }

    public class ARegistrations : IDotNetRegistrations
    {
        public void RegisterIn(IDotNetContainer container)
        {
        }
    }
    public class BRegistrations : IDotNetRegistrations
    {
        public void RegisterIn(IDotNetContainer container)
        {
        }
    }
}
