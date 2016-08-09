using DotNet.Basics.Diagnostics;
using NLog;
using NLog.Targets;
using NUnit.Framework;

namespace DotNet.Basics.Tests
{
    [SetUpFixture]
    public class TestInit
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            LogManager.ThrowExceptions = true;
            using (var logConfig = new NLogConfigurator())
            {
                logConfig.AddTarget(new ConsoleTarget("Console"));
                logConfig.Build();
            }
        }
    }
}
