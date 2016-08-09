using DotNet.Basics.Diagnostics;
using NLog.Targets;
using NUnit.Framework;

namespace DotNet.Basics.Tests
{
    [SetUpFixture]
    public class TestInit
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            using (var logConfig = new NLogConfigurator())
            {

                logConfig.AddTarget(new ConsoleTarget("Console"));
                logConfig.Build();
            }
        }
    }
}
