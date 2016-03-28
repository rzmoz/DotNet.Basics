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
        public void Update_UpdateParamAfterRegister()
        {
            var args = new TestCmdArgs();
            var cmd = new CmdLine()
                .Register<TestCmdArgs>(ref args,Required.No);

            cmd[nameof(TestCmdArgs.AncestorProp)].Required.Should().Be(Required.No);
            cmd[nameof(TestCmdArgs.AncestorProp)].Required = Required.Yes;
            cmd[nameof(TestCmdArgs.AncestorProp)].Required.Should().Be(Required.Yes);
        }

        [Test]
        [TestCase(ValueMode.Raw, "\"withQuotes\"", "\"withQuotes\"")]
        [TestCase(ValueMode.TrimForDoubleQuote, "\"withQuotes\"", "withQuotes")]
        public void ValueMode_SetValue_ValueModeIsConfigurable(ValueMode mode, string input, string expected)
        {
            string outValue = null;

            var cmd = new CmdLine(mode).
                Register(_paramName, Required.Yes, AllowEmpty.No, p => { outValue = p.Value; });

            outValue.Should().BeNull();

            cmd.Parse(new[] {$"-{_paramName}", input});

            outValue.Should().Be(expected);
        }
        
        [Test]
        public void Register_SimpleRegistration_ParamIsRegsitered()
        {
            var expectedValue = "MyExpectedValue";

            var args = new TestCmdArgs();
            var cmd = new CmdLine()
                .Register<TestCmdArgs>(ref args);
            cmd[nameof(TestCmdArgs.Prop1)].Required=Required.No;

            args.AncestorProp.Should().BeNull();

            var arg0 = $"-{nameof(TestCmdArgs.AncestorProp)}";
            cmd.Parse(new[] { arg0, expectedValue });

            args.AncestorProp.Should().Be(expectedValue);
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
