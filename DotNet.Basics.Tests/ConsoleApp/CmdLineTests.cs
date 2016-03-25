using System.Linq;
using DotNet.Basics.ConsoleApp;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.ConsoleApp
{
    [TestFixture]
    public class CmdLineTests
    {
        [Test]
        [TestCase("-")]//dash
        [TestCase("/")]//slash
        public void Parse_ParamNoValue_ParamExists(string paramIndicator)
        {
            const string paramName = "MyParam";

            var cmdLine = new CmdLine().Register(paramName, Required.Yes, AllowEmpty.Yes, param =>
            {
                param.Exists.Should().BeTrue();
            });

            var parseResult = cmdLine.Parse(new[] { $"{paramIndicator}{paramName}" });
            parseResult.Should().BeTrue();
        }

        [Test]
        public void Ctor_RegisterDebug_DebugIsAlreadyRegistered()
        {
            var cmd = new CmdLine();

            cmd.Count.Should().Be(1);
            cmd.Single().Name.Should().Be("debug");
            cmd["Debug"].Should().NotBeNull();
        }

        [Test]
        public void Clear_RegisterDebug_DebugIsAlwaysRegistered()
        {
            //arrange
            var cmd = new CmdLine();
            cmd.Count.Should().Be(1);
            cmd.Single().Name.Should().Be("debug");
            cmd["Debug"].Should().NotBeNull();

            cmd.Register("myParam", Required.No, AllowEmpty.No, param => { });
            cmd.Count.Should().Be(2);
            
            //act
            cmd.ClearParameters();

            //assert
            cmd.Count.Should().Be(1);
            cmd.Single().Name.Should().Be("debug");
            cmd["Debug"].Should().NotBeNull();
        }

        [Test]
        public void Index_GetUnregisteredParam_ParamNotFound()
        {
            var cmd = new CmdLine();
            cmd["DOESNTEXISTS"].Should().BeNull();
        }
    }
}
