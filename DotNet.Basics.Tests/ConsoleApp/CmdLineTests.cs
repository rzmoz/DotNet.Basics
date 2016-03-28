using System;
using System.Linq;
using DotNet.Basics.ConsoleApp;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.ConsoleApp
{
    [TestFixture]
    public class CmdLineTests
    {
        const string _paramName = "MyParam";

        [Test]
        [TestCase("-")]//dash
        [TestCase("/")]//slash
        public void Parse_ParamNoValue_ParamExists(string paramIndicator)
        {


            var cmdLine = new CmdLine().Register(_paramName, Required.Yes, AllowEmpty.Yes, param =>
            {
                param.Exists.Should().BeTrue();
            });

            var parseResult = cmdLine.Parse(new[] { $"{paramIndicator}{_paramName}" });
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
        public void Remove_RemmvingExistingParams_ParamIsRemoved()
        {
            var cmd = new CmdLine();

            cmd.Register(_paramName, Required.No, AllowEmpty.Yes, p => { });

            cmd.Count.Should().Be(2);//debug param exists already

            //register same param with different casing
            Action aciont = () => cmd.Register(_paramName.ToLower(), Required.No, AllowEmpty.Yes, p => { });
            aciont.ShouldThrow<ArgumentException>();

            cmd.Remove(_paramName.ToUpper());//remove with different casing than when registered

            cmd.Single().Name.Should().Be("debug");//assert only debug is left
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
        public void HelpScreen_Debug_DebugIsNotShownInHelpscreen()
        {
            var cmd = new CmdLine();
            cmd.HelpScreen().Trim('\n', '\r').Should().Be("Parameters:");
        }
        [Test]
        public void Index_GetUnregisteredParam_ParamNotFound()
        {
            var cmd = new CmdLine();
            cmd["DOESNTEXISTS"].Should().BeNull();
        }
    }
}
