using DotNet.Basics.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Tasks
{
    [TestFixture]
    public class BaseTaskTests
    {
        [Test]
        [TestCase("MyName")]//chars
        [TestCase("")]//empty str
        [TestCase(null)]//null
        public void Name_Ctor_NameIsSet(string name)
        {
            var task = new TestTask(name);
            task.Name.Should().Be(name ?? string.Empty);
        }
        [Test]
        [TestCase("MyName", "301bccdeca7744d20191425248ce32679073ac80f11448e01cf76ac6d86ec5e73897324693ce840e11bde969ba5eaf20c47fb7318841e26c080da79c93aa8ca0")]//chars
        [TestCase("", "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]//empty str
        [TestCase(null, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]//null
        public void Id_GenerateId_IdIsGenerated(string name, string expectedId)
        {
            var task = new TestTask(name);
            task.Id.Should().Be(expectedId);
        }

        [Test]
        public void Id_Casing_IdIsInsensitiveToNameCasing()
        {
            var rawName = "Id_Casing_IdIsInsensitiveToNameCasing";
            var upperName = rawName.ToUpper();
            var lowerName = rawName.ToLower();

            var taskWithUpperName = new TestTask(upperName);
            var taskWithLowerName = new TestTask(lowerName);
            taskWithLowerName.Id.Should().Be(taskWithUpperName.Id);
        }


        private class TestTask : TaskInfo
        {
            public TestTask(string name = null) : base(name)
            {
            }
        }
    }
}
