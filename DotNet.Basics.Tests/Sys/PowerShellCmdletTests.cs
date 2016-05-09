using System.Linq;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Sys
{
    [TestFixture]
    public class PowerShellCmdletTests
    {
        [Test]
        public void Parameters_ArrayParam_ValueIsArrayFormatting()
        {
            //arrange
            var paramKey = "myKey";
            var arrayValues = new[] { "myStr1", "myStr2", "myStr3" };
            var cmdlet = new PowerShellCmdlet(string.Empty);

            //act
            cmdlet.AddParameter(paramKey, arrayValues);

            //assert
            cmdlet.Parameters.Single().Value.ToString().Should().Be("(\"myStr1\",\"myStr2\",\"myStr3\")");
        }

        [Test]
        public void ToScript_Call_CmdletFormatted()
        {
            //arrange
            var cmdletName = "My-Cmdlet";
            var paramKey = "myKey";
            var paramValue = "myValue";
            var cmdlet = new PowerShellCmdlet(cmdletName);

            //act
            cmdlet.AddParameter(paramKey, paramValue); //full value
            cmdlet.AddParameter("Force"); //full value

            //assert
            cmdlet.ToScript().Should().Be($"{cmdletName} -{paramKey} \"{paramValue}\" -Force");
        }
    }
}
